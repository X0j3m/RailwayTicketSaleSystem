import React, {useEffect, useState} from 'react';
import {HubConnection, HubConnectionBuilder} from '@microsoft/signalr';
import {SignalRContext} from './SignalRContext';

export const SignalRProvider: React.FC<{ children: React.ReactNode }> = ({children}) => {
    const apiUrl = "http://localhost:5003/hub/app";

    const [connection, setConnection] = useState<HubConnection | null>(null);
    const [isConnected, setIsConnected] = useState(false);

    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl(apiUrl)
            .withAutomaticReconnect()
            .build();

        newConnection.start()
            .then(() => {
                console.log("Global SignalR connection started!");
                setConnection(newConnection);
                setIsConnected(true);
            })
            .catch(err => {
                console.error("SignalR error: ", err);
            });

        return () => {
            newConnection.stop();
        };
    }, []);

    return (
        <SignalRContext.Provider value={{connection, isConnected}}>
            {children}
        </SignalRContext.Provider>
    );
};