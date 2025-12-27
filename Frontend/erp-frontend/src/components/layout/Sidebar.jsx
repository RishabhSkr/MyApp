import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { LayoutDashboard, Factory, Box, Layers } from 'lucide-react';

const Sidebar = () => {
    const location = useLocation();

    const menuItems = [
        { path: '/', name: 'Dashboard', icon: LayoutDashboard },
        { path: '/production', name: 'Production Floor', icon: Factory },
        { path: '/inventory', name: 'Inventory', icon: Box },
        { path: '/masters', name: 'Master Data Management', icon: Layers },
    ];

    return (
        <div className="h-screen w-64 bg-slate-900 text-white fixed left-0 top-0">
            <div className="p-4 text-2xl font-bold border-b border-slate-700">
                ERP System
            </div>
            <nav className="mt-4">
                {menuItems.map((item) => {
                    const Icon = item.icon;
                    const isActive = location.pathname === item.path;
                    return (
                        <Link
                            key={item.path}
                            to={item.path}
                            className={`flex items-center gap-3 p-4 hover:bg-slate-800 transition-colors ${
                                isActive ? 'bg-blue-600 border-r-4 border-blue-400' : ''
                            }`}
                        >
                            <Icon size={20} />
                            <span>{item.name}</span>
                        </Link>
                    );
                })}
            </nav>
        </div>
    );
};

export default Sidebar;