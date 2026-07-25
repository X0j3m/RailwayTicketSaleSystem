import {StrictMode} from 'react'
import {createRoot} from 'react-dom/client'
import {SignalRProvider} from "./context/SignalRProvider.tsx";
import ConnectionStatus from "./components/ConnectionStatus/ConnectionStatus";
import TrainConnections from "./components/TrainConnections/TrainConnections.tsx";

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <SignalRProvider>
            <ConnectionStatus/>
            <TrainConnections/>
        </SignalRProvider>
    </StrictMode>
)
