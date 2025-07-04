import { test, expect } from '@playwright/test';

test.describe('Attendance Management', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/login');
    await page.fill('[data-testid=email-input]', 'test@example.com');
    await page.fill('[data-testid=password-input]', 'password123');
    await page.click('[data-testid=login-button]');
    await page.waitForURL('**/dashboard');
  });

  test('should check in successfully', async ({ page }) => {
    await page.goto('/attendance');
    
    await page.click('[data-testid=checkin-button]');
    
    await expect(page.locator('[data-testid=checkin-success]')).toBeVisible();
    await expect(page.locator('[data-testid=checkin-time]')).toBeVisible();
  });

  test('should check out successfully', async ({ page }) => {
    await page.goto('/attendance');
    
    await page.click('[data-testid=checkout-button]');
    
    await expect(page.locator('[data-testid=checkout-success]')).toBeVisible();
    await expect(page.locator('[data-testid=checkout-time]')).toBeVisible();
  });

  test('should display attendance history', async ({ page }) => {
    await page.goto('/attendance/history');
    
    await expect(page.locator('[data-testid=attendance-table]')).toBeVisible();
    await expect(page.locator('[data-testid=attendance-row]').first()).toBeVisible();
  });

  test('should filter attendance by date range', async ({ page }) => {
    await page.goto('/attendance/history');
    
    await page.fill('[data-testid=start-date]', '2024-01-01');
    await page.fill('[data-testid=end-date]', '2024-01-31');
    await page.click('[data-testid=filter-button]');
    
    await expect(page.locator('[data-testid=attendance-table]')).toBeVisible();
  });
});
