import React, { useState } from 'react'
import { useAuth } from '../contexts/AuthContext'
import ErrorBoundary from '../components/ErrorBoundary'

const AttendancePage: React.FC = () => {
  const { user } = useAuth()
  const [checkedIn, setCheckedIn] = useState(false)
  const [checkInTime, setCheckInTime] = useState<string | null>(null)
  const [checkOutTime, setCheckOutTime] = useState<string | null>(null)

  const handleCheckIn = () => {
    const now = new Date().toLocaleTimeString()
    setCheckedIn(true)
    setCheckInTime(now)
    setCheckOutTime(null)
  }

  const handleCheckOut = () => {
    const now = new Date().toLocaleTimeString()
    setCheckedIn(false)
    setCheckOutTime(now)
  }

  return (
    <ErrorBoundary>
      <div className="min-h-screen bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
        <div className="max-w-md mx-auto">
          <div className="bg-white shadow rounded-lg p-6">
            <h2 className="text-2xl font-bold text-gray-900 mb-6 text-center">
              Attendance Management
            </h2>
            
            <div className="space-y-4">
              <div className="text-center">
                <p className="text-sm text-gray-600">
                  Welcome, {user?.firstName} {user?.lastName}
                </p>
                <p className="text-xs text-gray-500">
                  Employee ID: {user?.employeeId}
                </p>
              </div>

              <div className="border-t pt-4">
                {!checkedIn ? (
                  <button
                    data-testid="checkin-button"
                    onClick={handleCheckIn}
                    className="w-full bg-green-600 text-white py-3 px-4 rounded-md text-lg font-medium hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500"
                  >
                    Check In
                  </button>
                ) : (
                  <button
                    data-testid="checkout-button"
                    onClick={handleCheckOut}
                    className="w-full bg-red-600 text-white py-3 px-4 rounded-md text-lg font-medium hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500"
                  >
                    Check Out
                  </button>
                )}
              </div>

              {checkInTime && (
                <div data-testid="checkin-success" className="bg-green-50 border border-green-200 rounded-md p-3">
                  <p className="text-sm text-green-800">
                    ✅ You have checked in successfully
                  </p>
                  <p data-testid="checkin-time" className="text-xs text-green-600">
                    Check-in time: {checkInTime}
                  </p>
                </div>
              )}

              {checkOutTime && (
                <div data-testid="checkout-success" className="bg-blue-50 border border-blue-200 rounded-md p-3">
                  <p className="text-sm text-blue-800">
                    ✅ You have checked out successfully
                  </p>
                  <p data-testid="checkout-time" className="text-xs text-blue-600">
                    Check-out time: {checkOutTime}
                  </p>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </ErrorBoundary>
  )
}

export default AttendancePage
