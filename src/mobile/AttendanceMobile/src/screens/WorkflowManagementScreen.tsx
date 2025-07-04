import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  Alert,
  ActivityIndicator,
} from 'react-native';
import { Colors } from '../utils/colors';
import { WorkflowEngineService, WorkflowDefinition, WorkflowInstance, WorkflowTask } from '../services/WorkflowEngineService';

const WorkflowManagementScreen: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [selectedTab, setSelectedTab] = useState('definitions');
  const [definitions, setDefinitions] = useState<WorkflowDefinition[]>([]);
  const [instances, setInstances] = useState<WorkflowInstance[]>([]);
  const [tasks, setTasks] = useState<WorkflowTask[]>([]);
  const workflowService = new WorkflowEngineService();

  useEffect(() => {
    loadData();
  }, [selectedTab]);

  const loadData = async () => {
    try {
      setLoading(true);
      if (selectedTab === 'definitions') {
        const definitionsData = await workflowService.getWorkflowDefinitions();
        setDefinitions(definitionsData);
      } else if (selectedTab === 'instances') {
        const instancesData = await workflowService.getActiveWorkflows();
        setInstances(instancesData);
      } else if (selectedTab === 'tasks') {
        const tasksData = await workflowService.getPendingTasks();
        setTasks(tasksData);
      }
    } catch (error) {
      Alert.alert('Error', 'Failed to load workflow data');
    } finally {
      setLoading(false);
    }
  };

  const startWorkflow = async (workflowId: string) => {
    try {
      await workflowService.startWorkflow(workflowId, {});
      Alert.alert('Success', 'Workflow started successfully');
      if (selectedTab === 'instances') {
        loadData();
      }
    } catch (error) {
      Alert.alert('Error', 'Failed to start workflow');
    }
  };

  const completeTask = async (taskId: string) => {
    try {
      await workflowService.completeTask(taskId, {});
      Alert.alert('Success', 'Task completed successfully');
      loadData();
    } catch (error) {
      Alert.alert('Error', 'Failed to complete task');
    }
  };

  const renderDefinition = ({ item }: { item: WorkflowDefinition }) => (
    <View style={styles.itemCard}>
      <Text style={styles.itemTitle}>{item.name}</Text>
      <Text style={styles.itemDescription}>{item.description}</Text>
      <View style={styles.itemMeta}>
        <Text style={styles.metaText}>Version: {item.version}</Text>
        <Text style={[styles.metaText, item.isActive ? styles.active : styles.inactive]}>
          {item.isActive ? 'Active' : 'Inactive'}
        </Text>
      </View>
      <TouchableOpacity
        style={styles.actionButton}
        onPress={() => startWorkflow(item.id)}
      >
        <Text style={styles.actionButtonText}>Start Workflow</Text>
      </TouchableOpacity>
    </View>
  );

  const renderInstance = ({ item }: { item: WorkflowInstance }) => (
    <View style={styles.itemCard}>
      <Text style={styles.itemTitle}>{item.workflowName || 'Workflow Instance'}</Text>
      <Text style={styles.itemDescription}>Current Step: {item.currentStep}</Text>
      <View style={styles.itemMeta}>
        <Text style={styles.metaText}>Status: {item.status}</Text>
        <Text style={styles.metaText}>
          Started: {new Date(item.startedAt).toLocaleDateString()}
        </Text>
      </View>
    </View>
  );

  const renderTask = ({ item }: { item: WorkflowTask }) => (
    <View style={styles.itemCard}>
      <Text style={styles.itemTitle}>{item.name}</Text>
      <Text style={styles.itemDescription}>{item.description}</Text>
      <View style={styles.itemMeta}>
        <Text style={styles.metaText}>Priority: {item.priority}</Text>
        <Text style={styles.metaText}>Status: {item.status}</Text>
      </View>
      {item.dueDate && (
        <Text style={styles.dueDateText}>
          Due: {new Date(item.dueDate).toLocaleDateString()}
        </Text>
      )}
      <TouchableOpacity
        style={styles.actionButton}
        onPress={() => completeTask(item.id)}
      >
        <Text style={styles.actionButtonText}>Complete Task</Text>
      </TouchableOpacity>
    </View>
  );

  const renderContent = () => {
    if (loading) {
      return (
        <View style={styles.loadingContainer}>
          <ActivityIndicator size="large" color={Colors.primary} />
          <Text style={styles.loadingText}>Loading...</Text>
        </View>
      );
    }

    switch (selectedTab) {
      case 'definitions':
        return (
          <FlatList
            data={definitions}
            renderItem={renderDefinition}
            keyExtractor={(item) => item.id}
            style={styles.list}
          />
        );
      case 'instances':
        return (
          <FlatList
            data={instances}
            renderItem={renderInstance}
            keyExtractor={(item) => item.id}
            style={styles.list}
          />
        );
      case 'tasks':
        return (
          <FlatList
            data={tasks}
            renderItem={renderTask}
            keyExtractor={(item) => item.id}
            style={styles.list}
          />
        );
      default:
        return null;
    }
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Workflow Management</Text>
      
      <View style={styles.tabBar}>
        <TouchableOpacity
          style={[styles.tab, selectedTab === 'definitions' && styles.activeTab]}
          onPress={() => setSelectedTab('definitions')}
        >
          <Text style={[styles.tabText, selectedTab === 'definitions' && styles.activeTabText]}>
            Definitions
          </Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={[styles.tab, selectedTab === 'instances' && styles.activeTab]}
          onPress={() => setSelectedTab('instances')}
        >
          <Text style={[styles.tabText, selectedTab === 'instances' && styles.activeTabText]}>
            Instances
          </Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={[styles.tab, selectedTab === 'tasks' && styles.activeTab]}
          onPress={() => setSelectedTab('tasks')}
        >
          <Text style={[styles.tabText, selectedTab === 'tasks' && styles.activeTabText]}>
            Tasks
          </Text>
        </TouchableOpacity>
      </View>

      {renderContent()}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    padding: 20,
    color: Colors.text,
  },
  tabBar: {
    flexDirection: 'row',
    backgroundColor: 'white',
    marginHorizontal: 16,
    borderRadius: 8,
    padding: 4,
  },
  tab: {
    flex: 1,
    paddingVertical: 12,
    alignItems: 'center',
    borderRadius: 6,
  },
  activeTab: {
    backgroundColor: Colors.primary,
  },
  tabText: {
    fontSize: 14,
    fontWeight: '600',
    color: Colors.textSecondary,
  },
  activeTabText: {
    color: 'white',
  },
  list: {
    flex: 1,
    padding: 16,
  },
  loadingContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  loadingText: {
    marginTop: 16,
    fontSize: 16,
    color: Colors.textSecondary,
  },
  itemCard: {
    backgroundColor: 'white',
    padding: 16,
    marginBottom: 12,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  itemTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 8,
    color: Colors.text,
  },
  itemDescription: {
    fontSize: 14,
    color: Colors.textSecondary,
    marginBottom: 12,
  },
  itemMeta: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: 12,
  },
  metaText: {
    fontSize: 12,
    color: Colors.textSecondary,
  },
  active: {
    color: Colors.success,
  },
  inactive: {
    color: Colors.error,
  },
  dueDateText: {
    fontSize: 12,
    color: Colors.warning,
    marginBottom: 12,
  },
  actionButton: {
    backgroundColor: Colors.primary,
    paddingVertical: 8,
    paddingHorizontal: 16,
    borderRadius: 6,
    alignSelf: 'flex-start',
  },
  actionButtonText: {
    color: 'white',
    fontSize: 14,
    fontWeight: '600',
  },
});

export default WorkflowManagementScreen;
