import { useState } from "react";
import { Routes, Route } from "react-router-dom";
import Topbar from "./scenes/shared/Topbar";
import SidebarComponent from "./scenes/shared/Sidebar";
import Home from "./scenes/shared/home";
import Login from "./scenes/auth/login";
import Dashboard from "./scenes/dashboard";
import Users from "./scenes/users";
import UserView from "./scenes/users/view";
import Profile from "./scenes/users/profile";
import Clients from "./scenes/clients";
import ClientView from "./scenes/clients/view";
import AddClient from "./scenes/clients/add";
import Devices from "./scenes/devices";
import AddDevice from "./scenes/devices/add";
import RegisterForm from "./scenes/users/register";
import FAQ from "./scenes/faq";
import { CssBaseline, ThemeProvider } from "@mui/material";
import { ColorModeContext, useMode } from "./theme";

import PrivateRoute from "./components/PrivateRoute";

const App = () => {
  const [theme, colorMode] = useMode();
  const [isSidebar, setIsSidebar] = useState(true);

  return (
    <ColorModeContext.Provider value={colorMode}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <div className="app">
          <SidebarComponent isSidebar={isSidebar} />
          <main className="content">
            <Topbar setIsSidebar={setIsSidebar} />
            <Routes>
              <Route path="/" element={<Home />} />
              <Route path="/login" element={<Login />} />
              <Route element={<PrivateRoute />}>
                <Route path="/dashboard" element={<Dashboard />} />
                <Route path="/users" element={<Users />} />
                <Route path="/user/:id" element={<UserView />} />
                <Route path="/profile" element={<Profile />} />
                <Route path="/clients" element={<Clients />} />
                <Route path="/client/:id" element={<ClientView />} />
                <Route path="/clients/add" element={<AddClient />} />
                <Route path="/devices" element={<Devices />} />
                <Route path="/devices/add" element={<AddDevice />} />
                <Route path="/register" element={<RegisterForm />} />
                <Route path="/faq" element={<FAQ />} />
              </Route>
            </Routes>
          </main>
        </div>
      </ThemeProvider>
    </ColorModeContext.Provider>
  );
}

export default App;
