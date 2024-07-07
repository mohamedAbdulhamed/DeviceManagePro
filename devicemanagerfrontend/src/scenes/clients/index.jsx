import { Box, Button, Typography, useTheme } from "@mui/material";
import { DataGrid, GridToolbar } from "@mui/x-data-grid";
import { tokens } from "../../theme";
import Header from "../../components/Header";
import React, { useState, useEffect } from "react";
import { getToken } from "../../utils/auth";
import axios, { AxiosError } from "axios";
import { useNavigate, Link } from "react-router-dom";


const Clients = () => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  const [clients, setClients] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const fetchClients = async () => {
    setLoading(true);
    try {
      const token = getToken();
      if (token === null) console.error("token is null");
      const response = await axios.get("https://localhost:7001/api/Client/GetAll", {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      const data = response.data.map(client => ({
        ...client,
        createdAt: new Date(client.createdAt + "T12:00:00"),
        updatedAt: new Date(client.updatedAt + "T12:00:00"),
      }));
      setClients(data);
      setLoading(false);
    } catch (err) {
      if (err && err instanceof AxiosError) setError(err.response?.data);
      else if (err && err instanceof Error) setError(err.message);
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchClients();
  }, []);

  const columns = [
    {
      field: "id",
      headerName: "ID",
      flex: 0.25,
      renderCell: (params) => (
        <Button
          component={Link}
          to={`/client/${params.value}`}
          variant="text"
          sx={{ color: colors.blueAccent[500] }}
        >
          {params.value}
        </Button>
      ),
    },    {
      field: "name",
      headerName: "Name",
      flex: 1,
      cellClassName: "name-column--cell",
    },
    {
      field: "nationalId",
      headerName: "National ID",
      flex: 1,
    },
    {
      field: "latitude",
      headerName: "Latitude",
      type: "number",
      headerAlign: "left",
      align: "left",
      flex: 0.5
    },
    {
      field: "longitude",
      headerName: "Longitude",
      type: "number",
      headerAlign: "left",
      align: "left",
      flex: 0.5
    },
    {
      field: "createdBy",
      headerName: "Created By",
      flex: 1,
    },
    {
      field: "createdAt",
      headerName: "Created At",
      type: "date",
      flex: 0.5,
    },
    {
      field: "updatedAt",
      headerName: "Updated At",
      type: "date",
      flex: 0.5,
    },
    {
      field: "actions",
      headerName: "Actions",
      flex: 1,
      renderCell: (params) => (
        <Button
          variant="contained"
          sx={{
            backgroundColor: colors.redAccent[500],
            color: colors.grey[100],
            borderRadius: "4px",
          }}
          onClick={() => handleDelete(params.row.id)}
        >
          Delete
        </Button>
      ),
    },
  ];

  const handleDelete = async (id) => {
    try {
      const token = getToken();
      if (token === null) navigate("/login");

      await axios.delete(`https://localhost:7001/api/Client/Delete/${id}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      fetchClients();
    } catch (err) {
      if (err && err instanceof AxiosError) setError(err.response?.data.error);
      else if (err && err instanceof Error) setError(err.message);
    }
  };

  if (loading) return <Typography>Loading...</Typography>;


  return (
    <Box m="20px">
      <Header
        title="Clients"
        subtitle="List of Clients for Future Reference"
      />

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
        <DataGrid
          rows={clients}
          columns={columns}
          components={{ Toolbar: GridToolbar }}
        />
      </Box>
    </Box>
  );
};

export default Clients;
