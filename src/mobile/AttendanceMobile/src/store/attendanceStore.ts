import { create } from 'zustand';
import { AttendanceRecord, AttendanceType, AttendanceMethod } from '../types/Attendance';

interface AttendanceState {
  records: AttendanceRecord[];
  currentStatus: 'checked_in' | 'checked_out' | 'on_break' | null;
  lastCheckIn: AttendanceRecord | null;
  lastCheckOut: AttendanceRecord | null;
  todayAttendance: AttendanceRecord | null;
  isLoading: boolean;
  error: string | null;
  
  addRecord: (record: AttendanceRecord) => void;
  setRecords: (records: AttendanceRecord[]) => void;
  updateCurrentStatus: () => void;
  setCurrentStatus: (status: 'checked_in' | 'checked_out' | 'on_break' | null) => void;
  setLoading: (loading: boolean) => void;
  setError: (error: string | null) => void;
  clearRecords: () => void;
  checkIn: (method: AttendanceMethod, location?: any, photo?: string) => Promise<void>;
  checkOut: (method: AttendanceMethod, location?: any, photo?: string) => Promise<void>;
  fetchTodayAttendance: () => Promise<void>;
}

export const useAttendanceStore = create<AttendanceState>((set, get) => ({
  records: [],
  currentStatus: null,
  lastCheckIn: null,
  lastCheckOut: null,
  todayAttendance: null,
  isLoading: false,
  error: null,
  
  addRecord: (record: AttendanceRecord) => {
    set(state => ({
      records: [record, ...state.records].sort((a, b) => 
        new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
      )
    }));
    get().updateCurrentStatus();
  },
  
  setRecords: (records: AttendanceRecord[]) => {
    const sortedRecords = records.sort((a, b) => 
      new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
    );
    set({ records: sortedRecords });
    get().updateCurrentStatus();
  },
  
  updateCurrentStatus: () => {
    const { records } = get();
    if (records.length === 0) {
      set({ currentStatus: null, lastCheckIn: null, lastCheckOut: null });
      return;
    }
    
    const latestRecord = records[0];
    const lastCheckIn = records.find(r => r.type === AttendanceType.CHECK_IN);
    const lastCheckOut = records.find(r => r.type === AttendanceType.CHECK_OUT);
    
    let currentStatus: 'checked_in' | 'checked_out' | 'on_break' | null = null;
    
    if (latestRecord.type === AttendanceType.CHECK_IN) {
      currentStatus = 'checked_in';
    } else if (latestRecord.type === AttendanceType.CHECK_OUT) {
      currentStatus = 'checked_out';
    } else if (latestRecord.type === AttendanceType.BREAK_START) {
      currentStatus = 'on_break';
    } else if (latestRecord.type === AttendanceType.BREAK_END) {
      currentStatus = 'checked_in';
    }
    
    set({
      currentStatus,
      lastCheckIn,
      lastCheckOut,
    });
  },
  
  setCurrentStatus: (status: 'checked_in' | 'checked_out' | 'on_break' | null) => {
    set({ currentStatus: status });
  },
  
  setLoading: (loading: boolean) => {
    set({ isLoading: loading });
  },
  
  setError: (error: string | null) => {
    set({ error });
  },
  
  clearRecords: () => {
    set({
      records: [],
      currentStatus: null,
      lastCheckIn: null,
      lastCheckOut: null,
      todayAttendance: null,
    });
  },
  
  checkIn: async (method: AttendanceMethod, location?: any, photo?: string) => {
    set({ isLoading: true, error: null });
    try {
      console.log('Check in with method:', method);
      set({ currentStatus: 'checked_in' });
    } catch (error) {
      set({ error: error instanceof Error ? error.message : 'Check-in failed' });
    } finally {
      set({ isLoading: false });
    }
  },
  
  checkOut: async (method: AttendanceMethod, location?: any, photo?: string) => {
    set({ isLoading: true, error: null });
    try {
      console.log('Check out with method:', method);
      set({ currentStatus: 'checked_out' });
    } catch (error) {
      set({ error: error instanceof Error ? error.message : 'Check-out failed' });
    } finally {
      set({ isLoading: false });
    }
  },
  
  fetchTodayAttendance: async () => {
    set({ isLoading: true, error: null });
    try {
      console.log('Fetching today attendance');
    } catch (error) {
      set({ error: error instanceof Error ? error.message : 'Failed to fetch attendance' });
    } finally {
      set({ isLoading: false });
    }
  },
}));
