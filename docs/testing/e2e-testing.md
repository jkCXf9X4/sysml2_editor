# E2E Testing

End-to-end tests should validate full user workflows through the web application in a real browser.

The recommended method is web-first browser testing with Playwright, not Electron. Electron should only be introduced when the product needs desktop-specific behavior such as native menus, local packaging, auto-update, or privileged filesystem workflows.

