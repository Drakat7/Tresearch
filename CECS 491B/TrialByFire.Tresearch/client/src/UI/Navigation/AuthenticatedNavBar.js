import React, { useEffect, useState} from "react";
import logo from './logo.png';
import { ContextMenu, ContextMenuTrigger, MenuItem, showMenu } from "react-contextmenu";
import axios from 'axios';
import jwt_decode from "jwt-decode";
import './AuthenticatedNavBar.css';


function AuthenticatedNavBar() {
    const [profileData, setProfileData] = useState([]);
    const [isNotPortal, setIsNotPortal] = useState(false);
    const [isNotSearch, setIsNotSearch] = useState(false);

    const CheckToken = () => {
      const token = sessionStorage.getItem('authorization');
      const decoded = jwt_decode(token);
      setProfileData(decoded.username[0]);
      if(window.location.pathname !== "/Portal")
      {
        setIsNotPortal(true);
      }
      if(window.location.pathname !== "/Search"){
        setIsNotSearch(true);
      }
      return;
    }

    useEffect(() => {
      CheckToken();
    }, [])


    const renderProfile = (e) => {
      const initial = profileData.toString().toUpperCase();  //Replace this later with initial from token
      return initial;
    }

    const handleLeftMouseClick = (e) => {
      const x = window.innerWidth-10;
      const y = 70;
      showMenu({
        position: {x, y},
        id: "contextmenu"
      });
    }
    
    const handleSettingsClick = (e) => {
        window.location = '/Settings';
    }

    const handlePortalClick = (e) => {
      window.location = '/Portal';
    }

    const handleFAQClick = (e) => {
      window.location = '/FAQ';
    }

    const handleLogoutClick = (e) => {
      e.preventDefault();
      axios.defaults.headers.common['Authorization'] = sessionStorage.getItem('authorization');
      axios.post('https://trialbyfiretresearchwebapi.azurewebsites.net/Logout/logout', {})
      .then(response => {

      }).catch(err => {
            
      })
      sessionStorage.removeItem('authorization');
      window.location = '/';
    }

    const handleSearchClick = (e) => {
      window.location = '/Search';
    }

    const renderMenu = (
        <div className = "nav-profile">
            <ContextMenuTrigger id = "contextmenu">
              <div className = "profile" onClick = {handleLeftMouseClick}>
                {renderProfile()}
              </div>
            </ContextMenuTrigger>
            <ContextMenu id = "contextmenu" className = "nav-context-menu">
              {isNotPortal ? <MenuItem onClick={handlePortalClick}>Portal</MenuItem> : null }
              {isNotSearch ? <MenuItem onClick={handleSearchClick}>Search</MenuItem> : null }
              <MenuItem onClick = {handleFAQClick}>FAQ</MenuItem>
              <MenuItem onClick={handleSettingsClick}>Settings</MenuItem>
              <MenuItem onClick={handleLogoutClick}>Logout</MenuItem>
            </ContextMenu>
        </div>
    );
    
    const renderNav = (
        <nav className = "authenticated-navbar-container">
           <ul className = "nav-links">
                <li className="logo"><a href="/" ><img src = {logo} alt = "Tresearch Logo"/></a></li>
                <li className="profile-container"><span>{renderMenu}</span></li>
            </ul>
        </nav>
    );
   
    return (
      <div className="authenticated-nav-bar-wrapper"> 
          {renderNav}
      </div>
    );
}

export default AuthenticatedNavBar;
