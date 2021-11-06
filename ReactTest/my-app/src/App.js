import logo from './logo.svg';
import './App.css';



import { FixedSizeList as List } from 'react-window';
 



function App() {

  const Row = ({ index, style }) => (
    <div style={style}>Row {index}</div>
  );
   
  const Example = () => (
    <List
      height={150}
      itemCount={1000}
      itemSize={35}
      width={300}
    >
      {Row}
    </List>
  );

  return (
    <div className="App">

    

    </div>
  );
}

export default App;
