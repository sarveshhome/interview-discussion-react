import useFetch from '../../../../core/api/useFetch';


function User() {

    const {
    data,
    loading,
    error
  } = useFetch('/users/1');

  if (loading) {
    return <h2>Loading...</h2>;
  }

  if (error) {
    return <h2>Something went wrong: {error.message}</h2>;
  }

  if (!data || data.length === 0) {
    return <h2>No users found</h2>;
  }

  return (
    <div>
      <h1>Users</h1>

      {data && data.map(user => (
        <div key={user.id}>
          <h3>{user.name}</h3>
          <p>{user.email}</p>
        </div>
      ))}
    </div>
  );
}

export default User;