namespace CatchyPlaywrightTests.Support
{
    public static class PlaywrightTestPages
    {
        public const string BasicTodoPageHtml = """
<!DOCTYPE html>
<html>
<head>
    <title>Playwright Test Page</title>
</head>
<body>
    <h1>todos</h1>
    <input id='todo-input' placeholder='What needs to be done?' />
    <button id='add-btn'>Add</button>
    <ul id='todo-list'>
        <li class='test-class'>Item 1</li>
    </ul>
    <div id='hidden-area' style='display:none'>Hidden text</div>
</body>
</html>
""";

        public const string DynamicBehaviorPageHtml = """
<!DOCTYPE html>
<html>
<head>
    <title>Dynamic Playwright Test Page</title>
</head>
<body>
    <h1 id='title'>loading...</h1>
    <input id='dynamic-input' value='' />
    <div id='late-element' style='display:none'>ready</div>

    <script>
        setTimeout(() => {
            document.getElementById('title').textContent = 'ready-title';
            document.getElementById('dynamic-input').value = 'ready-value';
            document.getElementById('late-element').style.display = 'block';
        }, 300);

        setTimeout(() => {
            document.getElementById('title').textContent = 'stable-title';
        }, 1100);
    </script>
</body>
</html>
""";
    }
}
