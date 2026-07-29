using System.Text.Json;
using Microsoft.Playwright;

namespace Catchy.Sdk
{
    public static class PwAccessibilityChecks
    {
        private const string LightweightScript = @"(() => {
            const issues = [];
            // images without alt or empty alt
            for (const img of document.images) {
                if (!img.hasAttribute('alt') || img.getAttribute('alt').trim() === '')
                    issues.push({type: 'image-alt', node: img.outerHTML});
            }
            // inputs without associated label or aria-label
            for (const inp of document.querySelectorAll('input, textarea, select')) {
                const id = inp.id;
                let hasLabel = false;
                if (id) hasLabel = !!document.querySelector(`label[for='${id}']`);
                if (!hasLabel && !inp.hasAttribute('aria-label') && !inp.hasAttribute('aria-labelledby'))
                    issues.push({type: 'form-control-label', node: inp.outerHTML});
            }
            // duplicate ids
            const ids = {};
            for (const el of document.querySelectorAll('[id]')) {
                const id = el.id;
                ids[id] = (ids[id] || 0) + 1;
            }
            for (const id in ids) if (ids[id] > 1) issues.push({type: 'duplicate-id', id, count: ids[id]});
            // landmarks existence
            const landmarks = ['banner','navigation','main','contentinfo','complementary','search'];
            let hasLandmark = false;
            for (const r of document.querySelectorAll('[role]')) if (landmarks.indexOf(r.getAttribute('role')) >= 0) { hasLandmark = true; break; }
            if (!hasLandmark) issues.push({type: 'landmark-missing'});
            // basic heading order (h1..h6 should not skip levels frequently)
            const headings = Array.from(document.querySelectorAll('h1,h2,h3,h4,h5,h6')).map(h=>parseInt(h.tagName.substring(1)));
            for (let i=1;i<headings.length;i++) if (headings[i] - headings[i-1] > 1) issues.push({type:'heading-skip', prev: headings[i-1], next: headings[i]});
            return {count: issues.length, issues};
        })()";

        public static CheckOperation Lightweight(IPage page, bool isSkipped, Func<float?> timeoutMs)
        {
            string? report = null;
            return CheckOperation.Async(async () =>
            {
                try
                {
                    // run quick checks in page context
                    var json = await page.EvaluateAsync<string>("() => JSON.stringify(" + LightweightScript + ")").ConfigureAwait(false);
                    report = json;
                    using var doc = JsonDocument.Parse(json ?? "{}");
                    var root = doc.RootElement;
                    int count = root.GetProperty("count").GetInt32();
                    return count == 0;
                }
                catch
                {
                    return false;
                }
            },
            () =>
            {
                if (report is null) return "Accessibility lightweight check failed to run";
                try
                {
                    using var doc = JsonDocument.Parse(report);
                    var root = doc.RootElement;
                    int count = root.GetProperty("count").GetInt32();
                    if (count == 0) return "No accessibility issues detected";
                    var issues = root.GetProperty("issues");
                    var first = issues[0].GetProperty("type").GetString();
                    return $"Found {count} accessibility issues (sample: {first})";
                }
                catch { return "Accessibility lightweight check failed to parse results"; }
            }, isSkipped);
        }

        public static CheckOperation Axe(IPage page, string axeScriptContent, bool isSkipped, Func<float?> timeoutMs)
        {
            string? report = null;
            return CheckOperation.Async(async () =>
            {
                try
                {
                    // inject axe script
                    await page.AddScriptTagAsync(new PageAddScriptTagOptions { Content = axeScriptContent }).ConfigureAwait(false);
                    // run axe with WCAG tags
                    var json = await page.EvaluateAsync<string>(@"async () => {
                        const opts = { runOnly: { type: 'tag', values: ['wcag2a','wcag2aa'] } };
                        const r = await axe.run(document, opts);
                        return JSON.stringify(r);
                    }").ConfigureAwait(false);
                    report = json;
                    using var doc = JsonDocument.Parse(json ?? "{}");
                    var violations = doc.RootElement.GetProperty("violations");
                    return violations.GetArrayLength() == 0;
                }
                catch
                {
                    return false;
                }
            },
            () =>
            {
                if (report is null) return "Axe accessibility check failed to run";
                try
                {
                    using var doc = JsonDocument.Parse(report);
                    var violations = doc.RootElement.GetProperty("violations");
                    int c = violations.GetArrayLength();
                    if (c == 0) return "No accessibility violations reported by axe";
                    var first = violations[0];
                    var id = first.GetProperty("id").GetString();
                    var nodes = first.GetProperty("nodes");
                    var target = nodes[0].GetProperty("target").EnumerateArray().FirstOrDefault().GetString();
                    return $"axe reported {c} violations (first: {id} on {target})";
                }
                catch { return "Axe accessibility check failed to parse results"; }
            }, isSkipped);
        }
    }
}
