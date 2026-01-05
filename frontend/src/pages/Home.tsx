import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';

export function Home() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center">
              <h1 className="text-xl font-bold text-gray-900">
                Transport System
              </h1>
            </div>
            <div className="flex items-center space-x-4">
              <span className="text-gray-700">
                Welcome, {user?.fullName}!
              </span>
              <span className="text-sm text-gray-500">
                ({user?.role})
              </span>
              <button
                onClick={handleLogout}
                className="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
              >
                Logout
              </button>
            </div>
          </div>
        </div>
      </nav>

      <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="bg-white shadow rounded-lg p-6">
            <h2 className="text-2xl font-bold text-gray-900 mb-4">
              Dashboard
            </h2>
            <div className="border-4 border-dashed border-gray-200 rounded-lg p-12">
              <div className="text-center">
                <h3 className="text-lg font-medium text-gray-900 mb-2">
                  Welcome to the Transport System
                </h3>
                <p className="text-gray-600">
                  Phase 1.2 - Authentication complete!
                </p>
                <p className="text-sm text-gray-500 mt-4">
                  Next phases will add resource management, work organization, and more features.
                </p>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}
