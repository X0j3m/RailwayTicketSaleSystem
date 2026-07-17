import 'leaflet/dist/leaflet.css';
import {StrictMode} from 'react'
import {createRoot} from 'react-dom/client'
import {SignalRProvider} from "./context/SignalRProvider.tsx";
import App from "./App.tsx";
import ConnectionStatus from "./components/ConnectionStatus/ConnectionStatus.tsx";
import {MapComponent} from "./components/MapComponent/MapComponent.tsx";

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <SignalRProvider>
            <ConnectionStatus/>
            <MapComponent/>
            <App/>
        </SignalRProvider>
    </StrictMode>,
)
