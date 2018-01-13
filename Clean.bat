@echo off
for %%i in (bin obj) do (
	echo %%i
	for /f %%j in ('dir /b /s %%i') do (
		del /f /s /q %%j
		RMDIR /s /q %%j
	)
)
