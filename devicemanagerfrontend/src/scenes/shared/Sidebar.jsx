import { useState } from "react";
import { ProSidebar, Menu, MenuItem } from "react-pro-sidebar";
import { Box, IconButton, Typography, useTheme } from "@mui/material";
import { Link, useNavigate } from "react-router-dom";
import { tokens } from "../../theme";
import { removeToken, removeRefreshToken } from '../../utils/auth';
import { useAuth } from "../../context/AuthContext";
import "react-pro-sidebar/dist/css/styles.css";
import HomeOutlinedIcon from "@mui/icons-material/HomeOutlined";
import PersonOutlinedIcon from "@mui/icons-material/PersonOutlined";
import PeopleIcon from "@mui/icons-material/People";
import HelpOutlineOutlinedIcon from "@mui/icons-material/HelpOutlineOutlined";
import MenuOutlinedIcon from "@mui/icons-material/MenuOutlined";
import ManageAccountsIcon from "@mui/icons-material/ManageAccounts";
import RouterIcon from "@mui/icons-material/Router";
import PersonAddAltIcon from "@mui/icons-material/PersonAddAlt";
import AddToQueueIcon from "@mui/icons-material/AddToQueue";
import ExitToAppIcon from "@mui/icons-material/ExitToApp";
import LoginIcon from '@mui/icons-material/Login';

const Item = ({ title, to, icon, selected, setSelected }) => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  return (
    <MenuItem
      active={selected === title}
      style={{
        color: colors.grey[100],
      }}
      onClick={() => setSelected(title)}
      icon={icon}
    >
      <Typography>{title}</Typography>
      <Link to={to} />
    </MenuItem>
  );
};

const Sidebar = () => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  const [isCollapsed, setIsCollapsed] = useState(false);
  const [selected, setSelected] = useState("Dashboard");
  const navigate = useNavigate();
  const { isAuthenticated, logout } = useAuth();

  const handleLogout = () => {
    removeToken();
    removeRefreshToken();
    logout();
    navigate("/login");
  };

  return (
    <Box
      sx={{
        "& .pro-sidebar-inner": {
          background: `${colors.primary[400]} !important`,
        },
        "& .pro-icon-wrapper": {
          backgroundColor: "transparent !important",
        },
        "& .pro-inner-item": {
          padding: "5px 35px 5px 20px !important",
        },
        "& .pro-inner-item:hover": {
          color: "#868dfb !important",
        },
        "& .pro-menu-item.active": {
          color: "#6870fa !important",
        },
      }}
    >
      <ProSidebar collapsed={isCollapsed}>
        <Menu iconShape="square">
          <MenuItem
            onClick={() => setIsCollapsed(!isCollapsed)}
            icon={isCollapsed ? <MenuOutlinedIcon /> : undefined}
            style={{
              margin: "10px 0 20px 0",
              color: colors.grey[100],
            }}
          >
            {!isCollapsed && (
              <Box
                display="flex"
                justifyContent="space-between"
                alignItems="center"
                ml="15px"
              >
                <Typography variant="h4" color={colors.grey[100]}>
                  Device Manager
                </Typography>
                <IconButton onClick={() => setIsCollapsed(!isCollapsed)}>
                  <MenuOutlinedIcon />
                </IconButton>
              </Box>
            )}
          </MenuItem>

          {isAuthenticated && (
            <>
              <Box mb="25px">
                <Box display="flex" justifyContent="center" alignItems="center">
                  {isCollapsed ? undefined : (
                    <img
                      alt="profile-user"
                      width="100px"
                      height="100px"
                      src={`../../assets/user.png`}
                      style={{ cursor: "pointer", borderRadius: "50%" }}
                    />
                  )}
                </Box>
                <Box textAlign="center">
                  <Typography
                    variant="h3"
                    color={colors.grey[100]}
                    fontWeight="bold"
                    sx={{ m: "10px 0 0 0" }}
                  >
                    {isCollapsed ? "M . M" : "Mohamed Magdy"}
                  </Typography>
                  <Typography variant="h5" color={colors.greenAccent[500]}>
                    Admin
                  </Typography>
                </Box>
              </Box>

              <Box paddingLeft={isCollapsed ? undefined : "10%"}>
                <Item
                  title="Dashboard"
                  to="/dashboard"
                  icon={<HomeOutlinedIcon />}
                  selected={selected}
                  setSelected={setSelected}
                />
                <Typography
                  variant="h6"
                  color={colors.grey[300]}
                  sx={{ m: "15px 0 5px 20px" }}
                >
                  Users
                </Typography>
                <Item
                  title="Manage Users"
                  to="/users"
                  icon={<ManageAccountsIcon />}
                  selected={selected}
                  setSelected={setSelected}
                />
                <Item
                  title="Register User"
                  to="/register"
                  icon={<PersonOutlinedIcon />}
                  selected={selected}
                  setSelected={setSelected}
                />
                <Typography
                  variant="h6"
                  color={colors.grey[300]}
                  sx={{ m: "15px 0 5px 20px" }}
                >
                  Clients
                </Typography>
                <Item
                  title="Manage Clients"
                  to="/clients"
                  icon={<PeopleIcon />}
                  selected={selected}
                  setSelected={setSelected}
                />
                <Item
                  title="Add Client"
                  to="/clients/add"
                  icon={<PersonAddAltIcon />}
                  selected={selected}
                  setSelected={setSelected}
                />
                <Typography
                  variant="h6"
                  color={colors.grey[300]}
                  sx={{ m: "15px 0 5px 20px" }}
                >
                  Devices
                </Typography>
                <Item
                  title="Manage Devices"
                  to="/devices"
                  icon={<RouterIcon />}
                  selected={selected}
                  setSelected={setSelected}
                />
                <Item
                  title="Add Device"
                  to="/devices/add"
                  icon={<AddToQueueIcon />}
                  selected={selected}
                  setSelected={setSelected}
                />
              </Box>
            </>
          )}

          <Box paddingLeft={isCollapsed ? undefined : "10%"}>
            <Typography
              variant="h6"
              color={colors.grey[300]}
              sx={{ m: "15px 0 5px 20px" }}
            >
              Help
            </Typography>
            <Item
              title="FAQ Page"
              to="/faq"
              icon={<HelpOutlineOutlinedIcon />}
              selected={selected}
              setSelected={setSelected}
            />

            {isAuthenticated && (
              <MenuItem
                onClick={handleLogout}
                icon={<ExitToAppIcon />}
                style={{
                  margin: "15px 0 5px 20px",
                  color: colors.redAccent[500],
                }}
              >
                <Typography>Logout</Typography>
              </MenuItem>
            )}

            {!isAuthenticated && (
              <Box paddingLeft={isCollapsed ? undefined : "10%"}>
                <Typography display={isCollapsed ? "none" : "block"}>Login to have more functionalities!</Typography>
                <Item
                  title="Login now"
                  to="/login"
                  icon={<LoginIcon />}
                  selected={selected}
                  setSelected={setSelected}
                />
              </Box>
            )}
          </Box>
        </Menu>
      </ProSidebar>
    </Box>
  );
};

export default Sidebar;
