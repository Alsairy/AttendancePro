import React from 'react'
import { Outlet, Link, useLocation } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'

const Layout: React.FC = () => {
  const { user, logout } = useAuth()
  const location = useLocation()

  const handleLogout = async () => {
    await logout()
  }

  const isActive = (path: string) => location.pathname === path

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-8">
              <Link to="/dashboard" className="text-xl font-semibold text-blue-600">
                Hudur Enterprise
              </Link>
              <div className="flex space-x-4">
                <Link
                  to="/dashboard"
                  className={`px-3 py-2 rounded-md text-sm font-medium ${
                    isActive('/dashboard')
                      ? 'bg-blue-100 text-blue-700'
                      : 'text-gray-500 hover:text-gray-700'
                  }`}
                >
                  Dashboard
                </Link>
                <Link
                  to="/attendance"
                  className={`px-3 py-2 rounded-md text-sm font-medium ${
                    isActive('/attendance')
                      ? 'bg-blue-100 text-blue-700'
                      : 'text-gray-500 hover:text-gray-700'
                  }`}
                >
                  Attendance
                </Link>
                <Link
                  to="/attendance/history"
                  className={`px-3 py-2 rounded-md text-sm font-medium ${
                    isActive('/attendance/history')
                      ? 'bg-blue-100 text-blue-700'
                      : 'text-gray-500 hover:text-gray-700'
                  }`}
                >
                  History
                </Link>
              </div>
            </div>
            <div className="flex items-center space-x-4">
              <div data-testid="user-menu" className="flex items-center space-x-2">
                <span className="text-sm text-gray-700">
                  Welcome, {user?.firstName} {user?.lastName}
                </span>
                <button
                  onClick={handleLogout}
                  className="bg-red-600 text-white px-3 py-1 rounded text-sm hover:bg-red-700"
                >
                  Logout
                </button>
              </div>
            </div>
          </div>
        </div>
      </nav>

      <main>
        <Outlet />
      </main>
    </div>
  )
}

export default Layout
