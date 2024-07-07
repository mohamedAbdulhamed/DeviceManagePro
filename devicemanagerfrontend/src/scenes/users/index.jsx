import React, { useState, useEffect } from "react";
import { Box, Typography, useTheme } from "@mui/material";
import { DataGrid, GridToolbar } from "@mui/x-data-grid";
import { tokens } from "../../theme";
import Header from "../../components/Header";
import { getToken } from "../../utils/auth"
import axios, { AxiosError } from "axios";

const Users = () => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const token = getToken();
        if (token === null) console.error("token is null");
        const response = await axios.get("https://localhost:7001/api/Account/GetUsers", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        const usersWithRoles = response.data.map(user => ({
          ...user,
          roles: user.roles.join(", ")
        }));
        setUsers(usersWithRoles);
        setLoading(false);
      } catch (err) {
        if (err && err instanceof AxiosError) setError(err.response?.data.error);
        else if (err && err instanceof Error) setError(err.message);
        setLoading(false);
      }
    };

    fetchUsers();
  }, []);

  const columns = [
    { field: "id", headerName: "ID" },
    {
      field: "name",
      headerName: "Name",
      flex: 1,
      cellClassName: "name-column--cell",
    },
    {
      field: "email",
      headerName: "Email",
      flex: 1,
    },
    {
      field: "roles",
      headerName: "Roles",
      flex: 1,
    }
  ];

  if (loading) return <Typography>Loading...</Typography>;

  return (
    <Box m="20px">
      <Header title="Users" subtitle="Managing users" />
      
      {error && <Typography color={colors.redAccent[500]} variant="h3" >{error}</Typography>}

      <Box
        m="40px 0 0 0"
        height="75vh"
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
        <DataGrid rows={users} columns={columns} components={{ Toolbar: GridToolbar }} />
      </Box>
    </Box>
  );
};

export default Users;
