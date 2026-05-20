const baseURL = "https://localhost:7017/api/";

export const fetcher = async (endpoint) => {
  let payload;
  let headers = {
    "Content-Type": "application/json",
  };

  // Add authorization header if we have a token
  const token = sessionStorage.getItem("token");
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  try {
    let response = await fetch(`${baseURL}${endpoint}`, {
      method: "GET",
      headers: headers,
    });
    payload = await response.json();
  } catch (error) {
    console.log(error);
    payload = { status: "Failed to fetch from server" };
  }
  return payload;
};
