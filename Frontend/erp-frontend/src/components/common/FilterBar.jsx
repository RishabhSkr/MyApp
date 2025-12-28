import React from 'react';
import { Search, X } from 'lucide-react';

const FilterBar = ({ 
    value,          // Input ka current value (State from parent)
    onChange,       // Value change hone par function (setState)
    onSearch,       // Search button click hone par function (API Call)
    onClear,        // Clear button click hone par function (Reset)
    placeholder = "Custom Search...", 
    type = "text"  
}) => {

    const handleSubmit = (e) => {
        e.preventDefault();
        onSearch(); // Parent ka search logic trigger karo
    };

    return (
        <form onSubmit={handleSubmit} className="flex flex-wrap gap-2 items-center w-full sm:w-auto">
            <div className="relative flex-1 sm:flex-none">
                {/* Input Field */}
                <input 
                    type={type} 
                    placeholder={placeholder} 
                    className="w-full sm:w-64 border border-gray-300 rounded-md px-3 py-2 pl-9 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 transition-shadow"
                    value={value}
                    onChange={(e) => onChange(e.target.value)}
                />
                {/* Search Icon inside Input */}
                <Search className="absolute left-2.5 top-2.5 text-gray-400" size={16} />
            </div>

            {/* Search Button */}
            <button 
                type="submit"
                className="bg-blue-600 text-white px-4 py-2 rounded-md text-sm font-medium hover:bg-blue-700 transition-colors flex items-center gap-2 shadow-sm"
            >
                Search
            </button>

            {/* Clear Button (Only visible if value exists) */}
            {value && (
                <button 
                    type="button"
                    onClick={onClear}
                    className="text-gray-500 hover:text-red-600 hover:bg-gray-100 px-3 py-2 rounded-md text-sm transition-colors flex items-center gap-1"
                >
                    <X size={16} /> Clear
                </button>
            )}
        </form>
    );
};

export default FilterBar;