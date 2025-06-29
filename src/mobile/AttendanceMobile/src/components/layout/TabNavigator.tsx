import React from 'react'
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native'

interface TabNavigatorProps {
  activeTab: string
  onTabChange: (tab: string) => void
}

const TabNavigator: React.FC<TabNavigatorProps> = ({ activeTab, onTabChange }) => {
  const tabs = [
    { id: 'dashboard', label: 'Dashboard' },
    { id: 'attendance', label: 'Attendance' },
    { id: 'history', label: 'History' },
    { id: 'profile', label: 'Profile' }
  ]

  return (
    <View style={styles.container}>
      {tabs.map((tab) => (
        <TouchableOpacity
          key={tab.id}
          style={[
            styles.tab,
            activeTab === tab.id && styles.activeTab
          ]}
          onPress={() => onTabChange(tab.id)}
        >
          <Text
            style={[
              styles.tabText,
              activeTab === tab.id && styles.activeTabText
            ]}
          >
            {tab.label}
          </Text>
        </TouchableOpacity>
      ))}
    </View>
  )
}

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    backgroundColor: '#fff',
    borderTopWidth: 1,
    borderTopColor: '#e5e5e5',
    paddingVertical: 8
  },
  tab: {
    flex: 1,
    alignItems: 'center',
    paddingVertical: 12
  },
  activeTab: {
    backgroundColor: '#f0f9ff'
  },
  tabText: {
    fontSize: 12,
    color: '#6b7280'
  },
  activeTabText: {
    color: '#2563eb',
    fontWeight: '600'
  }
})

export default TabNavigator
