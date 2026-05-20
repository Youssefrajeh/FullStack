const isProd = !["localhost", "127.0.0.1", "::1"].includes(window.location.hostname);
const serverURL = isProd ? '/api/' : 'https://localhost:7017/api/';
const fallbackURL = isProd ? '/api/' : 'http://localhost:5072/api/';

// Test if the server is reachable
const testConnection = async () => {
  try {
    let headers = buildHeaders()
    // Try HTTPS first
    const response = await fetch(`${serverURL}Brand`, {
      method: 'GET',
      mode: 'cors',
      headers: headers,
    })
    if (response.ok) {
      return true
    }

    // Try HTTP fallback
    const fallbackResponse = await fetch(`${fallbackURL}Brand`, {
      method: 'GET',
      mode: 'cors',
      headers: headers,
    })
    return fallbackResponse.ok
  } catch {
    // Try HTTP fallback on error
    try {
      let headers = buildHeaders()
      const fallbackResponse = await fetch(`${fallbackURL}Brand`, {
        method: 'GET',
        mode: 'cors',
        headers: headers,
      })
      return fallbackResponse.ok
    } catch {
      return false
    }
  }
}

const buildHeaders = () => {
  const myHeaders = new Headers()
  const customerData = sessionStorage.getItem('customer')
  if (customerData) {
    const customer = JSON.parse(customerData)
    myHeaders.append('Content-Type', 'application/json')
    myHeaders.append('Authorization', 'Bearer ' + customer.token)
  } else {
    myHeaders.append('Content-Type', 'application/json')
  }
  return myHeaders
}

const getApiUrl = async () => {
  try {
    let headers = buildHeaders()
    const response = await fetch(`${serverURL}Brand`, {
      method: 'GET',
      mode: 'cors',
      headers: headers,
    })
    if (response.ok) {
      return serverURL
    }
  } catch {
    // Continue to fallback
  }

  try {
    let headers = buildHeaders()
    const response = await fetch(`${fallbackURL}Brand`, {
      method: 'GET',
      mode: 'cors',
      headers: headers,
    })
    if (response.ok) {
      return fallbackURL
    }
  } catch {
    // Continue to default
  }

  return serverURL // Default fallback
}

const fetcher = async (endpoint) => {
  let payload
  let headers = buildHeaders()
  const apiUrl = await getApiUrl()

  try {
    const response = await fetch(`${apiUrl}${endpoint}`, {
      method: 'GET',
      headers: headers,
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    payload = await response.json()
  } catch (err) {
    payload = { error: `Error has occurred: ${err.message}` }
  }
  return payload
}

const poster = async (endpoint, dataToPost) => {
  let payload
  let headers = buildHeaders()
  const apiUrl = await getApiUrl()

  try {
    let response = await fetch(`${apiUrl}${endpoint}`, {
      method: 'POST',
      headers: headers,
      body: JSON.stringify(dataToPost),
    })

    if (!response.ok) {
      payload = { error: `HTTP error! status: ${response.status}` }
    } else {
      payload = await response.json()
    }
  } catch (error) {
    payload = { error: `Network error: ${error.message}` }
  }
  return payload
}

export { fetcher, testConnection, poster }
