import {StrictMode} from 'react'
import {createRoot} from 'react-dom/client'
import {SignalRProvider} from "./context/SignalRProvider.tsx";
import ConnectionStatus from "./components/ConnectionStatus/ConnectionStatus";
import RootComponent from "./components/RootComponent/RootComponent.tsx";

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <SignalRProvider>
            <ConnectionStatus/>
            <RootComponent/>
        </SignalRProvider>
    </StrictMode>
)
