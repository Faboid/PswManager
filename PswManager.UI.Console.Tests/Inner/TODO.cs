/*

Testing the bad paths of the AccountManager wrapping classes.

Why it's being delayed?
The CommandResult<T> class might replace its string-based error message in favor of a enum-based status result.
To properly test the bad paths, I plan to make sure the correct error codes are thrown out.
As it's currently still using the string-based error, there's no point adding those now.

*/