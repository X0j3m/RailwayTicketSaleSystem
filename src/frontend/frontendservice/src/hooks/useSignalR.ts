import {useContext} from 'react';
import {SignalRContext} from '../context/SignalRContext.ts';

export const useSignalR = () => {
    const context = useContext(SignalRContext);
    if (!context) {
        throw new Error("useSignalR must be used inside SignalRProvider!");
    }
    return context;
};