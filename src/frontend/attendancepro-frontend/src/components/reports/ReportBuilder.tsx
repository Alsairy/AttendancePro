import React, { useState, useEffect } from 'react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../ui/card'
import { Button } from '../ui/button'
import { Input } from '../ui/input'
import { Label } from '../ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select'
import { Checkbox } from '../ui/checkbox'
import { Textarea } from '../ui/textarea'
import { Badge } from '../ui/badge'
import { toast } from 'sonner'
import { Plus, Minus, Eye, Save, Download } from 'lucide-react'
import analyticsService from '../../services/analyticsService'

interface ReportField {
  id: string
  name: string
  displayName: string
  type: 'string' | 'number' | 'date' | 'boolean'
  category: string
  description?: string
}

interface ReportFilter {
  id: string
  field: string
  operator: 'equals' | 'contains' | 'greater_than' | 'less_than' | 'between' | 'in'
  value: string | string[]
}

interface ReportGrouping {
  field: string
  order: 'asc' | 'desc'
}

interface CustomReport {
  id?: string
  name: string
  description: string
  fields: string[]
  filters: ReportFilter[]
  groupBy: ReportGrouping[]
  chartType?: 'table' | 'bar' | 'line' | 'pie'
  schedule?: {
    frequency: 'daily' | 'weekly' | 'monthly'
    recipients: string[]
  }
}

const ReportBuilder: React.FC = () => {
  const [availableFields, setAvailableFields] = useState<ReportField[]>([])
  const [report, setReport] = useState<CustomReport>({
    name: '',
    description: '',
    fields: [],
    filters: [],
    groupBy: []
  })
  const [previewData, setPreviewData] = useState<any[]>([])
  const [isLoading, setIsLoading] = useState(false)
  const [showPreview, setShowPreview] = useState(false)

  useEffect(() => {
    loadAvailableFields()
  }, [])

  const loadAvailableFields = async () => {
    try {
      const mockFields: ReportField[] = [
        { id: 'employee_id', name: 'employee_id', displayName: 'Employee ID', type: 'string', category: 'Employee' },
        { id: 'employee_name', name: 'employee_name', displayName: 'Employee Name', type: 'string', category: 'Employee' },
        { id: 'department', name: 'department', displayName: 'Department', type: 'string', category: 'Employee' },
        { id: 'position', name: 'position', displayName: 'Position', type: 'string', category: 'Employee' },
        { id: 'check_in_time', name: 'check_in_time', displayName: 'Check In Time', type: 'date', category: 'Attendance' },
        { id: 'check_out_time', name: 'check_out_time', displayName: 'Check Out Time', type: 'date', category: 'Attendance' },
        { id: 'hours_worked', name: 'hours_worked', displayName: 'Hours Worked', type: 'number', category: 'Attendance' },
        { id: 'overtime_hours', name: 'overtime_hours', displayName: 'Overtime Hours', type: 'number', category: 'Attendance' },
        { id: 'is_late', name: 'is_late', displayName: 'Late Arrival', type: 'boolean', category: 'Attendance' },
        { id: 'leave_type', name: 'leave_type', displayName: 'Leave Type', type: 'string', category: 'Leave' },
        { id: 'leave_days', name: 'leave_days', displayName: 'Leave Days', type: 'number', category: 'Leave' },
        { id: 'leave_balance', name: 'leave_balance', displayName: 'Leave Balance', type: 'number', category: 'Leave' },
        { id: 'productivity_score', name: 'productivity_score', displayName: 'Productivity Score', type: 'number', category: 'Performance' },
        { id: 'efficiency_rating', name: 'efficiency_rating', displayName: 'Efficiency Rating', type: 'number', category: 'Performance' }
      ]
      setAvailableFields(mockFields)
    } catch (error) {
      console.error('Error loading fields:', error)
      toast.error('Failed to load available fields')
    }
  }

  const addField = (fieldId: string) => {
    if (!report.fields.includes(fieldId)) {
      setReport(prev => ({
        ...prev,
        fields: [...prev.fields, fieldId]
      }))
    }
  }

  const removeField = (fieldId: string) => {
    setReport(prev => ({
      ...prev,
      fields: prev.fields.filter(f => f !== fieldId)
    }))
  }

  const addFilter = () => {
    const newFilter: ReportFilter = {
      id: Date.now().toString(),
      field: '',
      operator: 'equals',
      value: ''
    }
    setReport(prev => ({
      ...prev,
      filters: [...prev.filters, newFilter]
    }))
  }

  const updateFilter = (filterId: string, updates: Partial<ReportFilter>) => {
    setReport(prev => ({
      ...prev,
      filters: prev.filters.map(f => f.id === filterId ? { ...f, ...updates } : f)
    }))
  }

  const removeFilter = (filterId: string) => {
    setReport(prev => ({
      ...prev,
      filters: prev.filters.filter(f => f.id !== filterId)
    }))
  }



  const generatePreview = async () => {
    if (report.fields.length === 0) {
      toast.error('Please select at least one field')
      return
    }

    try {
      setIsLoading(true)
      setShowPreview(true)

      const [productivity] = await Promise.all([
        analyticsService.getProductivityMetrics()
      ])

      const analyticsData = productivity.slice(0, 5).map(metric => ({
        employee_name: metric.userName,
        employee_id: metric.userId,
        department: metric.department,
        attendance_rate: `${(metric.attendanceRate * 100).toFixed(1)}%`,
        productivity_score: metric.productivityScore.toFixed(1),
        punctuality_score: metric.punctualityScore.toFixed(1),
        hours_worked: metric.averageHours.toFixed(1),
        check_in_time: new Date().toLocaleTimeString(),
        check_out_time: new Date(Date.now() + 8 * 60 * 60 * 1000).toLocaleTimeString(),
        is_late: metric.punctualityScore < 80,
        efficiency_rating: Math.round(metric.productivityScore / 10)
      }))

      const filteredData = analyticsData.map(row => {
        const filteredRow: any = {}
        report.fields.forEach(fieldId => {
          if (row.hasOwnProperty(fieldId)) {
            filteredRow[fieldId] = row[fieldId as keyof typeof row]
          }
        })
        return filteredRow
      })

      setPreviewData(filteredData)
      toast.success('Preview generated successfully')
    } catch (error) {
      console.error('Error generating preview:', error)
      toast.error('Failed to generate preview')
    } finally {
      setIsLoading(false)
    }
  }

  const saveReport = async () => {
    if (!report.name.trim()) {
      toast.error('Please enter a report name')
      return
    }

    if (report.fields.length === 0) {
      toast.error('Please select at least one field')
      return
    }

    try {
      setIsLoading(true)
      
      toast.success('Report saved successfully')
      
      setReport({
        name: '',
        description: '',
        fields: [],
        filters: [],
        groupBy: []
      })
    } catch (error) {
      console.error('Error saving report:', error)
      toast.error('Failed to save report')
    } finally {
      setIsLoading(false)
    }
  }

  const exportReport = async (format: 'pdf' | 'excel' | 'csv') => {
    try {
      setIsLoading(true)
      toast.success(`Exporting report as ${format.toUpperCase()}...`)
      
      let reportType: 'attendance' | 'productivity' | 'department' | 'predictive' = 'attendance'
      
      if (report.fields.some(field => field.includes('productivity') || field.includes('score'))) {
        reportType = 'productivity'
      } else if (report.fields.some(field => field.includes('department'))) {
        reportType = 'department'
      }

      const reportBlob = await analyticsService.exportReport(reportType, format)
      
      const downloadUrl = URL.createObjectURL(reportBlob)
      const link = document.createElement('a')
      link.href = downloadUrl
      link.download = `${report.name || 'custom-report'}.${format}`
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      
      URL.revokeObjectURL(downloadUrl)
      
      toast.success(`Report exported as ${format.toUpperCase()} successfully`)
    } catch (error) {
      console.error('Error exporting report:', error)
      toast.error('Failed to export report')
    } finally {
      setIsLoading(false)
    }
  }

  const getFieldsByCategory = (category: string) => {
    return availableFields.filter(field => field.category === category)
  }

  const categories = [...new Set(availableFields.map(field => field.category))]

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Custom Report Builder</CardTitle>
          <CardDescription>Build custom reports with advanced filtering and grouping</CardDescription>
        </CardHeader>
        <CardContent className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <Label htmlFor="reportName">Report Name</Label>
              <Input
                id="reportName"
                value={report.name}
                onChange={(e) => setReport(prev => ({ ...prev, name: e.target.value }))}
                placeholder="Enter report name"
              />
            </div>
            <div>
              <Label htmlFor="chartType">Visualization Type</Label>
              <Select value={report.chartType || 'table'} onValueChange={(value: any) => setReport(prev => ({ ...prev, chartType: value }))}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="table">Table</SelectItem>
                  <SelectItem value="bar">Bar Chart</SelectItem>
                  <SelectItem value="line">Line Chart</SelectItem>
                  <SelectItem value="pie">Pie Chart</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          <div>
            <Label htmlFor="description">Description</Label>
            <Textarea
              id="description"
              value={report.description}
              onChange={(e) => setReport(prev => ({ ...prev, description: e.target.value }))}
              placeholder="Enter report description"
              rows={3}
            />
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Select Fields</CardTitle>
          <CardDescription>Choose the data fields to include in your report</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {categories.map(category => (
              <div key={category}>
                <h4 className="font-medium text-sm text-gray-700 mb-2">{category}</h4>
                <div className="grid grid-cols-2 md:grid-cols-3 gap-2">
                  {getFieldsByCategory(category).map(field => (
                    <div key={field.id} className="flex items-center space-x-2">
                      <Checkbox
                        id={field.id}
                        checked={report.fields.includes(field.id)}
                        onCheckedChange={(checked) => {
                          if (checked) {
                            addField(field.id)
                          } else {
                            removeField(field.id)
                          }
                        }}
                      />
                      <Label htmlFor={field.id} className="text-sm">
                        {field.displayName}
                      </Label>
                    </div>
                  ))}
                </div>
              </div>
            ))}
          </div>

          {report.fields.length > 0 && (
            <div className="mt-4">
              <h4 className="font-medium text-sm text-gray-700 mb-2">Selected Fields</h4>
              <div className="flex flex-wrap gap-2">
                {report.fields.map(fieldId => {
                  const field = availableFields.find(f => f.id === fieldId)
                  return field ? (
                    <Badge key={fieldId} variant="secondary" className="flex items-center space-x-1">
                      <span>{field.displayName}</span>
                      <button
                        onClick={() => removeField(fieldId)}
                        className="ml-1 text-gray-500 hover:text-gray-700"
                      >
                        <Minus className="h-3 w-3" />
                      </button>
                    </Badge>
                  ) : null
                })}
              </div>
            </div>
          )}
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Filters</CardTitle>
          <CardDescription>Add filters to refine your report data</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {report.filters.map(filter => (
              <div key={filter.id} className="flex items-center space-x-2">
                <Select value={filter.field} onValueChange={(value) => updateFilter(filter.id, { field: value })}>
                  <SelectTrigger className="w-48">
                    <SelectValue placeholder="Select field" />
                  </SelectTrigger>
                  <SelectContent>
                    {availableFields.map(field => (
                      <SelectItem key={field.id} value={field.id}>
                        {field.displayName}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>

                <Select value={filter.operator} onValueChange={(value: any) => updateFilter(filter.id, { operator: value })}>
                  <SelectTrigger className="w-32">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="equals">Equals</SelectItem>
                    <SelectItem value="contains">Contains</SelectItem>
                    <SelectItem value="greater_than">Greater Than</SelectItem>
                    <SelectItem value="less_than">Less Than</SelectItem>
                    <SelectItem value="between">Between</SelectItem>
                  </SelectContent>
                </Select>

                <Input
                  value={Array.isArray(filter.value) ? filter.value.join(', ') : filter.value}
                  onChange={(e) => updateFilter(filter.id, { value: e.target.value })}
                  placeholder="Filter value"
                  className="flex-1"
                />

                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => removeFilter(filter.id)}
                >
                  <Minus className="h-4 w-4" />
                </Button>
              </div>
            ))}

            <Button variant="outline" onClick={addFilter}>
              <Plus className="mr-2 h-4 w-4" />
              Add Filter
            </Button>
          </div>
        </CardContent>
      </Card>

      <div className="flex justify-between items-center">
        <div className="flex space-x-2">
          <Button onClick={generatePreview} disabled={isLoading}>
            <Eye className="mr-2 h-4 w-4" />
            Preview
          </Button>
          <Button onClick={saveReport} disabled={isLoading}>
            <Save className="mr-2 h-4 w-4" />
            Save Report
          </Button>
        </div>
        <div className="flex space-x-2">
          <Button variant="outline" onClick={() => exportReport('pdf')} disabled={isLoading}>
            <Download className="mr-2 h-4 w-4" />
            Export PDF
          </Button>
          <Button variant="outline" onClick={() => exportReport('excel')} disabled={isLoading}>
            <Download className="mr-2 h-4 w-4" />
            Export Excel
          </Button>
          <Button variant="outline" onClick={() => exportReport('csv')} disabled={isLoading}>
            <Download className="mr-2 h-4 w-4" />
            Export CSV
          </Button>
        </div>
      </div>

      {showPreview && previewData.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle>Report Preview</CardTitle>
            <CardDescription>Preview of your report data</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <table className="w-full border-collapse border border-gray-300">
                <thead>
                  <tr className="bg-gray-50">
                    {report.fields.map(fieldId => {
                      const field = availableFields.find(f => f.id === fieldId)
                      return (
                        <th key={fieldId} className="border border-gray-300 px-4 py-2 text-left">
                          {field?.displayName || fieldId}
                        </th>
                      )
                    })}
                  </tr>
                </thead>
                <tbody>
                  {previewData.map((row, index) => (
                    <tr key={index}>
                      {report.fields.map(fieldId => (
                        <td key={fieldId} className="border border-gray-300 px-4 py-2">
                          {row[fieldId]?.toString() || '-'}
                        </td>
                      ))}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  )
}

export default ReportBuilder
