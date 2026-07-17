import {useSignalR} from "../../hooks/useSignalR.ts";
import {useEffect} from "react";

function ConnectionStatus() {

    const {connection, isConnected} = useSignalR();

    const myConnectionId = connection?.connectionId || '';

    useEffect(() => {
        if (!connection) return;
    }, [connection]);

    return (
        <div style={{padding: '20px', fontFamily: 'sans-serif'}}>
            <div style={{marginBottom: '20px'}}>
                <span>Server connection status: </span>
                <strong style={{color: isConnected ? 'green' : 'red'}}>
                    {isConnected ? '🟢 Connected' : '🔴 Disconnected'}
                </strong>
            </div>

            {isConnected && (
                <div style={{background: '#f0f0f0', padding: '10px', borderRadius: '5px', marginBottom: '20px'}}>
                    <strong>My unique ConnectionID:</strong>
                    <code style={{display: 'block', color: 'blue', marginTop: '5px'}}>
                        {myConnectionId || "Downloading ID..."}
                    </code>
                </div>
            )}
        </div>
    );
}

export default ConnectionStatus;