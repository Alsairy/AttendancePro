import React, { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  Alert,
  ActivityIndicator,
} from 'react-native';
import { Colors } from '../utils/colors';
import { VoiceRecognitionService } from '../services/VoiceRecognitionService';

const VoiceEnrollmentScreen: React.FC = () => {
  const [isRecording, setIsRecording] = useState(false);
  const [isProcessing, setIsProcessing] = useState(false);
  const voiceService = new VoiceRecognitionService();

  const handleStartEnrollment = async () => {
    try {
      setIsRecording(true);
      const audioData = await voiceService.startRecording();
      setIsRecording(false);
      setIsProcessing(true);
      
      const result = await voiceService.enrollVoice(audioData);
      setIsProcessing(false);
      
      if (result.success) {
        Alert.alert('Success', 'Voice enrolled successfully!');
      } else {
        Alert.alert('Error', result.message || 'Voice enrollment failed');
      }
    } catch (error) {
      setIsRecording(false);
      setIsProcessing(false);
      Alert.alert('Error', 'Voice enrollment failed');
    }
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Voice Enrollment</Text>
      <Text style={styles.subtitle}>
        Enroll your voice for secure authentication
      </Text>
      
      <View style={styles.instructionsContainer}>
        <Text style={styles.instructionText}>
          • Speak clearly in a quiet environment
        </Text>
        <Text style={styles.instructionText}>
          • Say your full name when prompted
        </Text>
        <Text style={styles.instructionText}>
          • Hold the device close to your mouth
        </Text>
      </View>

      <TouchableOpacity
        style={[styles.button, (isRecording || isProcessing) && styles.buttonDisabled]}
        onPress={handleStartEnrollment}
        disabled={isRecording || isProcessing}
      >
        {isProcessing ? (
          <ActivityIndicator color="white" />
        ) : (
          <Text style={styles.buttonText}>
            {isRecording ? 'Recording...' : 'Start Voice Enrollment'}
          </Text>
        )}
      </TouchableOpacity>

      {isRecording && (
        <View style={styles.recordingIndicator}>
          <Text style={styles.recordingText}>🎤 Recording...</Text>
        </View>
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    padding: 20,
    backgroundColor: Colors.background,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 16,
    color: Colors.text,
  },
  subtitle: {
    fontSize: 16,
    textAlign: 'center',
    marginBottom: 32,
    color: Colors.textSecondary,
  },
  instructionsContainer: {
    marginBottom: 32,
    alignSelf: 'stretch',
  },
  instructionText: {
    fontSize: 14,
    marginBottom: 8,
    color: Colors.textSecondary,
  },
  button: {
    backgroundColor: Colors.primary,
    paddingHorizontal: 32,
    paddingVertical: 16,
    borderRadius: 8,
    minWidth: 200,
    alignItems: 'center',
  },
  buttonDisabled: {
    backgroundColor: Colors.disabled,
  },
  buttonText: {
    color: 'white',
    fontSize: 16,
    fontWeight: '600',
  },
  recordingIndicator: {
    marginTop: 20,
    padding: 16,
    backgroundColor: Colors.error,
    borderRadius: 8,
  },
  recordingText: {
    color: 'white',
    fontSize: 16,
    fontWeight: 'bold',
  },
});

export default VoiceEnrollmentScreen;
