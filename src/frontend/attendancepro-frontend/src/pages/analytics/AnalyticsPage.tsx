import React, { useState, useEffect } from 'react'
import { TrendingUp, Activity, AlertTriangle, RefreshCw, Calendar, Users } from 'lucide-react'
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, BarChart, Bar } from 'recharts'
import { toast } from 'sonner'

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../components/ui/card'
import { Button } from '../../components/ui/button'
import { Badge } from '../../components/ui/badge'
import { Alert, AlertDescription } from '../../components/ui/alert'
import analyticsService, { AnalyticsOverview, AttendanceTrend, DepartmentAnalytics, PredictiveInsight, AnomalyDetection } from '../../services/analyticsService'



const AnalyticsPage: React.FC = () => {
  const [overview, setOverview] = useState<AnalyticsOverview | null>(null)
  const [trends, setTrends] = useState<AttendanceTrend[]>([])
  const [departments, setDepartments] = useState<DepartmentAnalytics[]>([])
  const [insights, setInsights] = useState<PredictiveInsight[]>([])
  const [anomalies, setAnomalies] = useState<AnomalyDetection[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [lastUpdated, setLastUpdated] = useState<Date>(new Date())

  const loadAnalyticsData = async () => {
    try {
      setIsLoading(true)
      const endDate = new Date()
      const startDate = new Date()
      startDate.setDate(endDate.getDate() - 30)

      const filter = {
        startDate: startDate.toISOString().split('T')[0],
        endDate: endDate.toISOString().split('T')[0]
      }

      const [overviewData, trendsData, departmentsData, insightsData, anomaliesData] = await Promise.all([
        analyticsService.getOverview(filter),
        analyticsService.getAttendanceTrends(filter),
        analyticsService.getDepartmentAnalytics(undefined, filter),
        analyticsService.getPredictiveInsights(undefined, 5),
        analyticsService.getAnomalies(false, undefined)
      ])

      setOverview(overviewData)
      setTrends(trendsData)
      setDepartments(departmentsData)
      setInsights(insightsData)
      setAnomalies(anomaliesData)
      setLastUpdated(new Date())
    } catch (error: any) {
      toast.error('Failed to load analytics data: ' + error.message)
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadAnalyticsData()
  }, [])

  const formatPercentage = (value: number): string => {
    return `${(value * 100).toFixed(1)}%`
  }

  const formatDate = (dateString: string): string => {
    return new Date(dateString).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })
  }

  const getInsightBadgeColor = (impact: string) => {
    switch (impact) {
      case 'high': return 'destructive'
      case 'medium': return 'default'
      case 'low': return 'secondary'
      default: return 'outline'
    }
  }

  const getAnomalySeverityColor = (severity: string) => {
    switch (severity) {
      case 'high': return 'text-red-600'
      case 'medium': return 'text-yellow-600'
      case 'low': return 'text-blue-600'
      default: return 'text-gray-600'
    }
  }

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Analytics</h1>
            <p className="text-muted-foreground">Loading insights and trends...</p>
          </div>
        </div>
        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4">
          {[1, 2, 3, 4].map((i) => (
            <Card key={i}>
              <CardContent className="p-6">
                <div className="animate-pulse">
                  <div className="h-4 bg-gray-200 rounded w-3/4 mb-2"></div>
                  <div className="h-8 bg-gray-200 rounded w-1/2"></div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Analytics</h1>
          <p className="text-muted-foreground">
            Real-time insights and trends from attendance data
          </p>
        </div>
        <div className="flex items-center gap-2">
          <span className="text-sm text-muted-foreground">
            Last updated: {lastUpdated.toLocaleTimeString()}
          </span>
          <Button variant="outline" size="sm" onClick={loadAnalyticsData} disabled={isLoading}>
            <RefreshCw className="h-4 w-4 mr-2" />
            Refresh
          </Button>
        </div>
      </div>

      {/* Key Metrics */}
      <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Attendance Rate</CardTitle>
            <TrendingUp className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{overview ? formatPercentage(overview.attendanceRate) : '--'}</div>
            <p className="text-xs text-muted-foreground">
              {overview ? `${overview.presentToday}/${overview.totalEmployees} present today` : 'Loading...'}
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Average Hours</CardTitle>
            <Activity className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{overview ? overview.averageWorkingHours.toFixed(1) : '--'}</div>
            <p className="text-xs text-muted-foreground">
              Per employee per day
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Absent Today</CardTitle>
            <Users className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{overview ? overview.absentToday : '--'}</div>
            <p className="text-xs text-muted-foreground">
              {overview ? `${overview.lateToday} late arrivals` : 'Loading...'}
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Leave Requests</CardTitle>
            <Calendar className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{overview ? overview.pendingLeaveRequests : '--'}</div>
            <p className="text-xs text-muted-foreground">
              {overview ? `${overview.upcomingLeaves} upcoming leaves` : 'Loading...'}
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Anomaly Alerts */}
      {anomalies.length > 0 && (
        <Alert>
          <AlertTriangle className="h-4 w-4" />
          <AlertDescription>
            <div className="flex items-center justify-between">
              <span>{anomalies.length} anomalies detected requiring attention</span>
              <Badge variant="destructive">{anomalies.filter(a => a.severity === 'high').length} High Priority</Badge>
            </div>
          </AlertDescription>
        </Alert>
      )}

      {/* Charts */}
      <div className="grid gap-6 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Attendance Trends</CardTitle>
            <CardDescription>Daily attendance patterns over the last 30 days</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="h-[300px]">
              <ResponsiveContainer width="100%" height="100%">
                <LineChart data={trends}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="date" tickFormatter={formatDate} />
                  <YAxis />
                  <Tooltip labelFormatter={(value) => `Date: ${formatDate(value as string)}`} />
                  <Legend />
                  <Line type="monotone" dataKey="present" stroke="#22c55e" strokeWidth={2} name="Present" />
                  <Line type="monotone" dataKey="absent" stroke="#ef4444" strokeWidth={2} name="Absent" />
                  <Line type="monotone" dataKey="late" stroke="#f59e0b" strokeWidth={2} name="Late" />
                </LineChart>
              </ResponsiveContainer>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Department Breakdown</CardTitle>
            <CardDescription>Attendance rates by department</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="h-[300px]">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={departments}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="departmentName" />
                  <YAxis tickFormatter={(value) => `${(value * 100).toFixed(0)}%`} />
                  <Tooltip formatter={(value: number) => [`${(value * 100).toFixed(1)}%`, 'Attendance Rate']} />
                  <Bar dataKey="attendanceRate" fill="#3b82f6" />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Predictive Insights */}
      {insights.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle>Predictive Insights</CardTitle>
            <CardDescription>AI-powered recommendations and forecasts</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {insights.map((insight, index) => (
                <div key={index} className="flex items-start justify-between p-4 border rounded-lg">
                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-2">
                      <h4 className="font-medium">{insight.title}</h4>
                      <Badge variant={getInsightBadgeColor(insight.impact)}>{insight.impact} impact</Badge>
                      <Badge variant="outline">{(insight.confidence * 100).toFixed(0)}% confidence</Badge>
                    </div>
                    <p className="text-sm text-muted-foreground mb-2">{insight.description}</p>
                    {insight.recommendedActions.length > 0 && (
                      <div className="text-xs">
                        <strong>Recommended actions:</strong>
                        <ul className="list-disc list-inside mt-1">
                          {insight.recommendedActions.slice(0, 2).map((action, i) => (
                            <li key={i}>{action}</li>
                          ))}
                        </ul>
                      </div>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      )}

      {/* Anomaly Detection */}
      {anomalies.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle>Anomaly Detection</CardTitle>
            <CardDescription>Unusual patterns and behaviors detected</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {anomalies.slice(0, 5).map((anomaly) => (
                <div key={anomaly.id} className="flex items-center justify-between p-3 border rounded-lg">
                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-1">
                      <span className={`text-sm font-medium ${getAnomalySeverityColor(anomaly.severity)}`}>
                        {anomaly.severity.toUpperCase()}
                      </span>
                      <span className="text-sm text-muted-foreground">
                        {new Date(anomaly.detectedAt).toLocaleDateString()}
                      </span>
                    </div>
                    <p className="text-sm">{anomaly.description}</p>
                    {anomaly.userName && (
                      <p className="text-xs text-muted-foreground">User: {anomaly.userName}</p>
                    )}
                  </div>
                  <Badge variant={anomaly.resolved ? 'secondary' : 'destructive'}>
                    {anomaly.resolved ? 'Resolved' : 'Active'}
                  </Badge>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  )
}

export default AnalyticsPage
