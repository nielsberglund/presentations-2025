const { useState, useEffect } = React;

function CatFactGenerator() {
  const [catFact, setCatFact] = useState("Click the button to get a random cat fact!");
  const [isLoading, setIsLoading] = useState(false);
  
  const fetchCatFact = async () => {
    try {
      setIsLoading(true);
      const response = await fetch('https://catfact.ninja/fact');
      const data = await response.json();
      setCatFact(data.fact);
    } catch (error) {
      console.error('Error fetching cat fact:', error);
      setCatFact('Failed to fetch a cat fact. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };
  
  return (
    <div style={{ 
      maxWidth: '600px', 
      margin: '0 auto', 
      padding: '20px',
      fontFamily: 'Arial, sans-serif',
      textAlign: 'center',
      backgroundColor: '#f9f9f9',
      borderRadius: '10px',
      boxShadow: '0 4px 8px rgba(0,0,0,0.1)'
    }}>
      <h1 style={{ color: '#333' }}>Random Cat Facts</h1>
      
      <div style={{ 
        backgroundColor: 'white',
        padding: '20px',
        borderRadius: '5px',
        minHeight: '100px',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        marginBottom: '20px'
      }}>
        {isLoading ? (
          <p>Loading...</p>
        ) : (
          <p style={{ fontSize: '18px' }}>{catFact}</p>
        )}
      </div>
      
      <button 
        onClick={fetchCatFact}
        disabled={isLoading}
        style={{
          backgroundColor: '#4CAF50',
          color: 'white',
          padding: '12px 20px',
          border: 'none',
          borderRadius: '4px',
          cursor: 'pointer',
          fontSize: '16px',
          transition: 'background-color 0.3s'
        }}
      >
        {isLoading ? 'Fetching...' : 'Get New Cat Fact'}
      </button>
    </div>
  );
}

// Render the component to the DOM
const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(<CatFactGenerator />);