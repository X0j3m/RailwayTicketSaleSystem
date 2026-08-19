import {StrictMode} from 'react'
import {createRoot} from 'react-dom/client'
import {SignalRProvider} from "./context/SignalRProvider.tsx";
import ConnectionStatus from "./components/ConnectionStatus/ConnectionStatus";
import App from "./App.tsx";

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <SignalRProvider>
            <ConnectionStatus/>
            <App/>
        </SignalRProvider>
    </StrictMode>
)
