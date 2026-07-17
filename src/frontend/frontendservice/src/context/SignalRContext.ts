import {createContext} from 'react';
import {HubConnection} from '@microsoft/signalr';

export interface SignalRContextType {
    connection: HubConnection | null;
    isConnected: boolean;
}

export const SignalRContext = createContext<SignalRContextType | undefined>(undefined);