import {useSignalR} from "../../hooks/useSignalR.ts";
import {type ChangeEvent, useEffect, useState} from "react";

function ConnectionStatus() {
    const {connection, isConnected} = useSignalR();
    const [devMode, setDevMode] = useState<boolean>(false);

    const myConnectionId = connection?.connectionId || '';

    useEffect(() => {
        if (!connection) return;
    }, [connection]);

    function handleChange(e: ChangeEvent<HTMLInputElement>) {
        setDevMode(e.target.checked);
    }

    return (
        <div>
            <label style={{userSelect: "none"}}>
                <input type="checkbox"
                       checked={devMode}
                       onChange={handleChange}/>
                Dev mode
            </label>

            {devMode &&
                <div style={{padding: '20px', fontFamily: 'sans-serif'}}>
                    <div style={{marginBottom: '20px', userSelect: "none"}}>
                        <span>Server connection status: </span>
                        <strong style={{color: isConnected ? 'green' : 'red'}}>
                            {isConnected ? '🟢 Connected' : '🔴 Disconnected'}
                        </strong>
                    </div>

                    {isConnected && (
                        <div>
                            <strong style={{userSelect: "none"}}>ConnectionID:</strong>
                            <code style={{display: 'block', color: 'blue', marginTop: '5px'}}>
                                {myConnectionId || "Downloading ID..."}
                            </code>
                        </div>
                    )}
                </div>}
        </div>

    );
}

export default ConnectionStatus;