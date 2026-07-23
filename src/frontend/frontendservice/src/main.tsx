import {StrictMode} from 'react'
import {createRoot} from 'react-dom/client'
import {SignalRProvider} from "./context/SignalRProvider.tsx";
import App from "./App.tsx";
import ConnectionStatus from "./components/ConnectionStatus/ConnectionStatus.tsx";
// import MapComponent from "./components/MapComponent/MapComponent.tsx";
import SearchConnectionComponent from "./components/SearchConnectionComponent/SearchConnectionComponent.tsx";

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <SignalRProvider>
            <ConnectionStatus/>
            <SearchConnectionComponent/>
            {/*<MapComponent/>*/}
            <App/>
        </SignalRProvider>
    </StrictMode>,
)
