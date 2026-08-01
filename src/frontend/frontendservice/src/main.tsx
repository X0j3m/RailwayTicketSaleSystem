import {StrictMode} from 'react'
import {createRoot} from 'react-dom/client'
import {SignalRProvider} from "./context/SignalRProvider.tsx";
import ConnectionStatus from "./components/ConnectionStatus/ConnectionStatus";
import TrainConnections from "./components/TrainConnections/TrainConnections.tsx";
import TrainScheme from "./components/TrainScheme/TrainScheme.tsx";

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <SignalRProvider>
            <TrainScheme/>
            <ConnectionStatus/>
            <TrainConnections/>
        </SignalRProvider>
    </StrictMode>
)
