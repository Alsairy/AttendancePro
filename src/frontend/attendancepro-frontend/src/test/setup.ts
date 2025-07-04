import { chromium, FullConfig } from '@playwright/test';

async function globalSetup(_config: FullConfig) {
  const browser = await chromium.launch();
  const context = await browser.newContext();
  const page = await context.newPage();
  
  try {
    await page.goto('http://localhost:3000');
    await page.waitForLoadState('networkidle');
  } catch (error) {
    console.log('Setup page load failed, continuing with tests');
  }
  
  await browser.close();
}

export default globalSetup;
