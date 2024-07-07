import { Box, useTheme, Button, Typography } from "@mui/material";
import { DataGrid, GridToolbar } from "@mui/x-data-grid";
import { tokens } from "../../theme";
import React, { useState, useEffect } from "react";
import { getToken } from "../../utils/auth"
import axios, { AxiosError } from "axios";
import { useNavigate } from "react-router-dom";
import StatBox from "../../components/StatBox";
import PeopleIcon from '@mui/icons-material/People';
import ManageAccountsIcon from '@mui/icons-material/ManageAccounts';
import RouterIcon from '@mui/icons-material/Router';
import DeleteIcon from '@mui/icons-material/Delete';

const Dashboard = () => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  const [actionLogs, setActionLogs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const fetchActionLogs = async () => {
    setLoading(true);
    try {
      const token = getToken();
      if (token === null) console.error("token is null");
      const response = await axios.get("https://localhost:7001/api/Account/GetLogs", {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      const data = response.data.map(log => ({
        ...log,
        timestamp: new Date(log.timestamp),
      }));
      setActionLogs(data);
      setLoading(false);
    } catch (err) {
      if (err && err instanceof AxiosError) setError(err.response?.data);
      else if (err && err instanceof Error) setError(err.message);
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchActionLogs();
  }, []);

  const columns = [
    { field: "id", headerName: "ID" },
    { field: "userId", headerName: "User ID", flex: 1 },
    { field: "userName", headerName: "User Name", flex: 1 },
    { field: "userRole", headerName: "Role", flex: 1 },
    { field: "action", headerName: "Action", flex: 1 },
    { field: "entity", headerName: "Entity", flex: 1 },
    { field: "statusCode", headerName: "Status", flex: 1 },
    { field: "timestamp", headerName: "Date", type: "date", flex: 1 },
    {
      field: "delete",
      headerName: "Delete",
      flex: 0.5,
      renderCell: (params) => (
        <Button
          onClick={() => handleDelete(params.row.id)}
          sx={{
            backgroundColor: colors.redAccent[500],
            color: colors.grey[100],
            borderRadius: "4px",
          }}
        >
          <DeleteIcon />
        </Button>
      ),
    },
  ];

  const handleDelete = async (id) => {
    return;
    // try {
    //   const token = getToken();
    //   if (token === null) navigate("/login");

    //   await axios.delete(`https://localhost:7001/api/Action/DeleteLog/${id}`, { // not yet implemented in the backend
    //     headers: {
    //       Authorization: `Bearer ${token}`,
    //     },
    //   });
    //   fetchActionLogs();
    // } catch (err) {
    //   if (err && err instanceof AxiosError) setError(err.response?.data.error);
    //   else if (err && err instanceof Error) setError(err.message);
    // }
  };

  if (loading) return <Typography>Loading...</Typography>;

  return (
    <Box m="20px">
      
      {error && <Typography color={colors.redAccent[500]} variant="h3" >{error}</Typography>}

      {/* Stats & Logs */}
      <Box
        display="grid"
        gridTemplateColumns="repeat(12, 1fr)"
        gridAutoRows="140px"
        gap="20px"
        sx={{
          "& .MuiBox-root.css-1yg19ie": {
            display: "none !important",
          },
        }}
      >
        {/* ROW 1 (Stats) */}
        <Box
          gridColumn="span 3"
          backgroundColor={colors.primary[400]}
          display="flex"
          alignItems="center"
          justifyContent="center"
        >
          <StatBox
            title="12,361"
            subtitle="Users"
            icon={
              <ManageAccountsIcon
                sx={{ color: colors.greenAccent[600], fontSize: "26px" }}
              />
            }
          />
        </Box>

        <Box
          gridColumn="span 3"
          backgroundColor={colors.primary[400]}
          display="flex"
          alignItems="center"
          justifyContent="center"
        >
          <StatBox
            title="431,225"
            subtitle="Clients"
            icon={
              <PeopleIcon
                sx={{ color: colors.greenAccent[600], fontSize: "26px" }}
              />
            }
          />
        </Box>

        <Box
          gridColumn="span 3"
          backgroundColor={colors.primary[400]}
          display="flex"
          alignItems="center"
          justifyContent="center"
        >
          <StatBox
            title="32,441"
            subtitle="Devices"
            icon={
              <RouterIcon
                sx={{ color: colors.greenAccent[600], fontSize: "26px" }}
              />
            }
          />
        </Box>

        {/* ROW 2 (Logs) */}
        <Box
          gridColumn="span 12"
          gridRow="span 3"
          backgroundColor={colors.primary[400]}
          overflow="auto"
          sx={{
            "& .MuiDataGrid-root": {
              border: "none",
            },
            "& .MuiDataGrid-cell": {
              borderBottom: "none",
            },
            "& .name-column--cell": {
              color: colors.greenAccent[300],
            },
            "& .MuiDataGrid-columnHeaders": {
              backgroundColor: colors.blueAccent[700],
              borderBottom: "none",
            },
            "& .MuiDataGrid-virtualScroller": {
              backgroundColor: colors.primary[400],
            },
            "& .MuiDataGrid-footerContainer": {
              borderTop: "none",
              backgroundColor: colors.blueAccent[700],
            },
            "& .MuiCheckbox-root": {
              color: `${colors.greenAccent[200]} !important`,
            },
            "& .MuiDataGrid-toolbarContainer .MuiButton-text": {
              color: `${colors.grey[100]} !important`,
            },
          }}
        >
          <DataGrid
            rows={actionLogs}
            columns={columns}
            res
            components={{ Toolbar: GridToolbar }}
          />
        </Box>
      </Box>
    </Box>
  );
};

export default Dashboard;
