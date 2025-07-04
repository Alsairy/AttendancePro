export const AuthService = {
  login: jest.fn().mockResolvedValue({
    token: 'mock-token',
    user: { 
      id: '1', 
      firstName: 'John', 
      lastName: 'Doe', 
      email: 'john@example.com',
      isActive: true,
      tenantId: 'tenant-1',
      createdAt: '2024-01-01T00:00:00Z'
    },
    refreshToken: 'refresh-token',
    expiresAt: '2024-01-01T01:00:00Z'
  })
}
