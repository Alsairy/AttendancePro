import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  TextInput,
  Alert,
} from 'react-native';
import { Colors } from '../utils/colors';
import { CollaborationService } from '../services/CollaborationService';

interface Team {
  id: string;
  name: string;
  description: string;
  memberCount: number;
}

interface ChatMessage {
  id: string;
  content: string;
  userId: string;
  createdAt: string;
}

const TeamCollaborationScreen: React.FC = () => {
  const [teams, setTeams] = useState<Team[]>([]);
  const [selectedTeam, setSelectedTeam] = useState<Team | null>(null);
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [newMessage, setNewMessage] = useState('');
  const [loading, setLoading] = useState(true);
  const collaborationService = new CollaborationService();

  useEffect(() => {
    loadTeams();
  }, []);

  const loadTeams = async () => {
    try {
      const teamsData = await collaborationService.getTeams();
      setTeams(teamsData);
    } catch (error) {
      Alert.alert('Error', 'Failed to load teams');
    } finally {
      setLoading(false);
    }
  };

  const loadMessages = async (teamId: string) => {
    try {
      const messagesData = await collaborationService.getMessages(teamId);
      setMessages(messagesData);
    } catch (error) {
      Alert.alert('Error', 'Failed to load messages');
    }
  };

  const sendMessage = async () => {
    if (!newMessage.trim() || !selectedTeam) return;

    try {
      await collaborationService.sendMessage({
        teamId: selectedTeam.id,
        content: newMessage,
        messageType: 'text',
      });
      setNewMessage('');
      loadMessages(selectedTeam.id);
    } catch (error) {
      Alert.alert('Error', 'Failed to send message');
    }
  };

  const selectTeam = (team: Team) => {
    setSelectedTeam(team);
    loadMessages(team.id);
  };

  const renderTeam = ({ item }: { item: Team }) => (
    <TouchableOpacity
      style={[
        styles.teamItem,
        selectedTeam?.id === item.id && styles.selectedTeam,
      ]}
      onPress={() => selectTeam(item)}
    >
      <Text style={styles.teamName}>{item.name}</Text>
      <Text style={styles.teamDescription}>{item.description}</Text>
      <Text style={styles.memberCount}>{item.memberCount} members</Text>
    </TouchableOpacity>
  );

  const renderMessage = ({ item }: { item: ChatMessage }) => (
    <View style={styles.messageItem}>
      <Text style={styles.messageContent}>{item.content}</Text>
      <Text style={styles.messageTime}>
        {new Date(item.createdAt).toLocaleTimeString()}
      </Text>
    </View>
  );

  if (!selectedTeam) {
    return (
      <View style={styles.container}>
        <Text style={styles.title}>Team Collaboration</Text>
        <FlatList
          data={teams}
          renderItem={renderTeam}
          keyExtractor={(item) => item.id}
          style={styles.teamsList}
        />
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity
          style={styles.backButton}
          onPress={() => setSelectedTeam(null)}
        >
          <Text style={styles.backButtonText}>← Back</Text>
        </TouchableOpacity>
        <Text style={styles.teamTitle}>{selectedTeam.name}</Text>
      </View>

      <FlatList
        data={messages}
        renderItem={renderMessage}
        keyExtractor={(item) => item.id}
        style={styles.messagesList}
      />

      <View style={styles.messageInput}>
        <TextInput
          style={styles.textInput}
          value={newMessage}
          onChangeText={setNewMessage}
          placeholder="Type a message..."
          multiline
        />
        <TouchableOpacity style={styles.sendButton} onPress={sendMessage}>
          <Text style={styles.sendButtonText}>Send</Text>
        </TouchableOpacity>
      </View>
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
  teamsList: {
    flex: 1,
    padding: 16,
  },
  teamItem: {
    backgroundColor: 'white',
    padding: 16,
    marginBottom: 8,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  selectedTeam: {
    borderColor: Colors.primary,
    backgroundColor: Colors.primaryLight,
  },
  teamName: {
    fontSize: 18,
    fontWeight: 'bold',
    color: Colors.text,
  },
  teamDescription: {
    fontSize: 14,
    color: Colors.textSecondary,
    marginTop: 4,
  },
  memberCount: {
    fontSize: 12,
    color: Colors.textSecondary,
    marginTop: 8,
  },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: 16,
    borderBottomWidth: 1,
    borderBottomColor: Colors.border,
  },
  backButton: {
    marginRight: 16,
  },
  backButtonText: {
    color: Colors.primary,
    fontSize: 16,
  },
  teamTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    color: Colors.text,
  },
  messagesList: {
    flex: 1,
    padding: 16,
  },
  messageItem: {
    backgroundColor: 'white',
    padding: 12,
    marginBottom: 8,
    borderRadius: 8,
    alignSelf: 'flex-end',
    maxWidth: '80%',
  },
  messageContent: {
    fontSize: 16,
    color: Colors.text,
  },
  messageTime: {
    fontSize: 12,
    color: Colors.textSecondary,
    marginTop: 4,
    textAlign: 'right',
  },
  messageInput: {
    flexDirection: 'row',
    padding: 16,
    borderTopWidth: 1,
    borderTopColor: Colors.border,
    alignItems: 'flex-end',
  },
  textInput: {
    flex: 1,
    borderWidth: 1,
    borderColor: Colors.border,
    borderRadius: 8,
    padding: 12,
    marginRight: 8,
    maxHeight: 100,
  },
  sendButton: {
    backgroundColor: Colors.primary,
    paddingHorizontal: 16,
    paddingVertical: 12,
    borderRadius: 8,
  },
  sendButtonText: {
    color: 'white',
    fontWeight: 'bold',
  },
});

export default TeamCollaborationScreen;
