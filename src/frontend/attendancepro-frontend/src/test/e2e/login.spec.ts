import { test, expect } from '@playwright/test';

test.describe.configure({ mode: 'parallel' });

test.describe('Login Flow', () => {
  test('should login successfully with valid credentials', async ({ page }) => {
    await page.goto('/login');
    
    await page.fill('[data-testid=email-input]', 'test@example.com');
    await page.fill('[data-testid=password-input]', 'password123');
    await page.click('[data-testid=login-button]');
    
    await expect(page).toHaveURL(/.*dashboard/);
    await expect(page.locator('[data-testid=user-menu]')).toBeVisible();
  });

  test('should show error with invalid credentials', async ({ page }) => {
    await page.goto('/login');
    
    await page.fill('[data-testid=email-input]', 'invalid@example.com');
    await page.fill('[data-testid=password-input]', 'wrongpassword');
    await page.click('[data-testid=login-button]');
    
    await expect(page.locator('[data-testid=error-message]')).toBeVisible();
    await expect(page.locator('[data-testid=error-message]')).toContainText('Invalid credentials');
  });

  test('should validate required fields', async ({ page }) => {
    await page.goto('/login');
    
    await page.click('[data-testid=login-button]');
    
    await expect(page.locator('[data-testid=email-error]')).toBeVisible();
    await expect(page.locator('[data-testid=password-error]')).toBeVisible();
  });
});
