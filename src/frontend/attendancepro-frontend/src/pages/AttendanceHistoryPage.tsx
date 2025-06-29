import React from 'react'
import ErrorBoundary from '../components/ErrorBoundary'

const AttendanceHistoryPage: React.FC = () => {
  const mockData = [
    {
      id: '1',
      date: '2024-01-15',
      checkInTime: '08:00:00',
      checkOutTime: '17:00:00',
      status: 'Present',
      workingHours: '09:00:00'
    },
    {
      id: '2',
      date: '2024-01-14',
      checkInTime: '08:15:00',
      checkOutTime: '17:30:00',
      status: 'Present',
      workingHours: '09:15:00'
    }
  ]

  return (
    <ErrorBoundary>
      <div className="min-h-screen bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
        <div className="max-w-4xl mx-auto">
          <div className="bg-white shadow rounded-lg">
            <div className="px-6 py-4 border-b border-gray-200">
              <h2 className="text-2xl font-bold text-gray-900">
                Attendance History
              </h2>
            </div>
            
            <div className="p-6">
              <div className="mb-4 flex space-x-4">
                <input
                  data-testid="start-date"
                  type="date"
                  className="border border-gray-300 rounded-md px-3 py-2"
                  placeholder="Start Date"
                />
                <input
                  data-testid="end-date"
                  type="date"
                  className="border border-gray-300 rounded-md px-3 py-2"
                  placeholder="End Date"
                />
                <button
                  data-testid="filter-button"
                  className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700"
                >
                  Filter
                </button>
              </div>

              <div className="overflow-x-auto">
                <table data-testid="attendance-table" className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Date
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Check In
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Check Out
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Status
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Working Hours
                      </th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {mockData.map((record) => (
                      <tr key={record.id} data-testid="attendance-row">
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {record.date}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {record.checkInTime}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {record.checkOutTime}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <span className="inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-green-100 text-green-800">
                            {record.status}
                          </span>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {record.workingHours}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      </div>
    </ErrorBoundary>
  )
}

export default AttendanceHistoryPage
