using Microsoft.Playwright;

namespace Catchy.Sdk
{
    public static class PwPerformanceChecks
    {
        public static CheckOperation LoadTimeLessThan(IPage page, double ms, bool not, bool isSkipped)
        {
            double loadTime = 0;
            return CheckOperation.Async(async () =>
            {
                loadTime = await page.EvaluateAsync<double>(
                    "() => { const t = performance.timing; return t.loadEventEnd - t.navigationStart; }").ConfigureAwait(false);
                bool ok = loadTime < ms;
                return not ? !ok : ok;
            },
            () => not
                ? $"Expected load time not to be < {ms}ms, but was {loadTime:F0}ms"
                : $"Expected load time < {ms}ms, but was {loadTime:F0}ms",
            isSkipped);
        }

        public static CheckOperation DomContentLoadedLessThan(IPage page, double ms, bool not, bool isSkipped)
        {
            double elapsed = 0;
            return CheckOperation.Async(async () =>
            {
                elapsed = await page.EvaluateAsync<double>(
                    "() => { const t = performance.timing; return t.domContentLoadedEventEnd - t.navigationStart; }").ConfigureAwait(false);
                bool ok = elapsed < ms;
                return not ? !ok : ok;
            },
            () => not
                ? $"Expected DOMContentLoaded not to be < {ms}ms, but was {elapsed:F0}ms"
                : $"Expected DOMContentLoaded < {ms}ms, but was {elapsed:F0}ms",
            isSkipped);
        }

        public static CheckOperation TimeToFirstByteLessThan(IPage page, double ms, bool not, bool isSkipped)
        {
            double ttfb = 0;
            return CheckOperation.Async(async () =>
            {
                ttfb = await page.EvaluateAsync<double>(
                    "() => { const t = performance.timing; return t.responseStart - t.requestStart; }").ConfigureAwait(false);
                bool ok = ttfb < ms;
                return not ? !ok : ok;
            },
            () => not
                ? $"Expected TTFB not to be < {ms}ms, but was {ttfb:F0}ms"
                : $"Expected TTFB < {ms}ms, but was {ttfb:F0}ms",
            isSkipped);
        }

        /// <summary>Checks paint timing (LCP, FCP) via PerformanceObserver entries.</summary>
        public static CheckOperation LargestContentfulPaintLessThan(IPage page, double ms, bool not, bool isSkipped)
        {
            double lcp = 0;
            return CheckOperation.Async(async () =>
            {
                lcp = await page.EvaluateAsync<double>(@"() => {
                    return new Promise(resolve => {
                        let v = 0;
                        const obs = new PerformanceObserver(list => {
                            for (const e of list.getEntries()) v = e.startTime;
                        });
                        obs.observe({ type: 'largest-contentful-paint', buffered: true });
                        setTimeout(() => { obs.disconnect(); resolve(v); }, 100);
                    });
                }").ConfigureAwait(false);
                bool ok = lcp < ms;
                return not ? !ok : ok;
            },
            () => not
                ? $"Expected LCP not to be < {ms}ms, but was {lcp:F0}ms"
                : $"Expected LCP < {ms}ms, but was {lcp:F0}ms",
            isSkipped);
        }

        public static CheckOperation FirstContentfulPaintLessThan(IPage page, double ms, bool not, bool isSkipped)
        {
            double fcp = 0;
            return CheckOperation.Async(async () =>
            {
                fcp = await page.EvaluateAsync<double>(@"() => {
                    const entries = performance.getEntriesByName('first-contentful-paint');
                    return entries.length > 0 ? entries[0].startTime : 0;
                }").ConfigureAwait(false);
                bool ok = fcp < ms;
                return not ? !ok : ok;
            },
            () => not
                ? $"Expected FCP not to be < {ms}ms, but was {fcp:F0}ms"
                : $"Expected FCP < {ms}ms, but was {fcp:F0}ms",
            isSkipped);
        }

        public static CheckOperation CumulativeLayoutShiftBelow(IPage page, double threshold, bool not, bool isSkipped)
        {
            double cls = 0;
            return CheckOperation.Async(async () =>
            {
                cls = await page.EvaluateAsync<double>(@"() => {
                    return new Promise(resolve => {
                        let v = 0;
                        const obs = new PerformanceObserver(list => {
                            for (const e of list.getEntries()) if (!e.hadRecentInput) v += e.value;
                        });
                        obs.observe({ type: 'layout-shift', buffered: true });
                        setTimeout(() => { obs.disconnect(); resolve(v); }, 100);
                    });
                }").ConfigureAwait(false);
                bool ok = cls < threshold;
                return not ? !ok : ok;
            },
            () => not
                ? $"Expected CLS not to be < {threshold}, but was {cls:F4}"
                : $"Expected CLS < {threshold}, but was {cls:F4}",
            isSkipped);
        }

        public static CheckOperation TotalBlockingTimeLessThan(IPage page, double ms, bool not, bool isSkipped)
        {
            double tbt = 0;
            return CheckOperation.Async(async () =>
            {
                tbt = await page.EvaluateAsync<double>(@"() => {
                    return new Promise(resolve => {
                        let total = 0;
                        const obs = new PerformanceObserver(list => {
                            for (const e of list.getEntries()) total += e.duration > 50 ? e.duration - 50 : 0;
                        });
                        obs.observe({ type: 'longtask', buffered: true });
                        setTimeout(() => { obs.disconnect(); resolve(total); }, 100);
                    });
                }").ConfigureAwait(false);
                bool ok = tbt < ms;
                return not ? !ok : ok;
            },
            () => not
                ? $"Expected TBT not to be < {ms}ms, but was {tbt:F0}ms"
                : $"Expected TBT < {ms}ms, but was {tbt:F0}ms",
            isSkipped);
        }

        public static CheckOperation ResourceCountLessThan(IPage page, string resourceType, int count, bool not, bool isSkipped)
        {
            int actual = 0;
            return CheckOperation.Async(async () =>
            {
                actual = await page.EvaluateAsync<int>(
                    $"() => performance.getEntriesByType('resource').filter(r => r.initiatorType === '{resourceType}').length").ConfigureAwait(false);
                bool ok = actual < count;
                return not ? !ok : ok;
            },
            () => not
                ? $"Expected {resourceType} resource count not to be < {count}, but was {actual}"
                : $"Expected {resourceType} resource count < {count}, but was {actual}",
            isSkipped);
        }

        public static CheckOperation NavigationTimingMetricLessThan(IPage page, string metric, double ms, bool not, bool isSkipped)
        {
            double elapsed = 0;
            return CheckOperation.Async(async () =>
            {
                elapsed = await page.EvaluateAsync<double>(
                    $"() => {{ const t = performance.timing; return t.{metric}; }}").ConfigureAwait(false);
                bool ok = elapsed < ms;
                return not ? !ok : ok;
            },
            () => not
                ? $"Expected performance.timing.{metric} not to be < {ms}ms, but was {elapsed:F0}ms"
                : $"Expected performance.timing.{metric} < {ms}ms, but was {elapsed:F0}ms",
            isSkipped);
        }
    }
}
