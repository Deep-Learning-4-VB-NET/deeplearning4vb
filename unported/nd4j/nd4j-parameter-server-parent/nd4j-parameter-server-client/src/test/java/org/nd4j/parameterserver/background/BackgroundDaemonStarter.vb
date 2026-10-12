Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ParameterServerSubscriber = org.nd4j.parameterserver.ParameterServerSubscriber
Imports ProcessExecutor = org.zeroturnaround.exec.ProcessExecutor

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.parameterserver.background



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BackgroundDaemonStarter
	Public Class BackgroundDaemonStarter


		''' <summary>
		'''  Start a slave daemon with
		'''  the specified master url with the form of:
		'''  hostname:port:streamId </summary>
		''' <param name="parameterLength"> the length of the parameters to
		'''                        be averaging
		''' @return </param>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="InterruptedException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int startSlave(int parameterLength, String masterUrl, String mediaDriverDirectory) throws Exception
		Public Shared Function startSlave(ByVal parameterLength As Integer, ByVal masterUrl As String, ByVal mediaDriverDirectory As String) As Integer
			Return exec(GetType(ParameterServerSubscriber), mediaDriverDirectory, "-s", "1," & parameterLength.ToString(), "-p", "40126", "-h", "localhost", "-id", "10", "-pm", masterUrl, "-sp", "9500", "--updatesPerEpoch", "1")
		End Function

		''' 
		''' <summary>
		''' Start a slave daemon with a default url of:
		''' localhost:40123:11
		''' where the url is:
		''' hostname:port:streamId </summary>
		''' <param name="parameterLength"> the parameter length of the ndarrays
		''' @return </param>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="InterruptedException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int startSlave(int parameterLength, String mediaDriverDirectory) throws Exception
		Public Shared Function startSlave(ByVal parameterLength As Integer, ByVal mediaDriverDirectory As String) As Integer
			Return startSlave(parameterLength, "localhost:40123:11", mediaDriverDirectory)
		End Function


		Public Shared Function slaveConnectionUrl() As String
			Return "localhost:40126:10"
		End Function

		''' <summary>
		''' Master connection url
		''' @return
		''' </summary>
		Public Shared Function masterResponderUrl() As String
			Return "localhost:40124:12"
		End Function

		''' <summary>
		''' Master connection url
		''' @return
		''' </summary>
		Public Shared Function masterConnectionUrl() As String
			Return "localhost:40123:11"
		End Function

		''' 
		''' <param name="parameterLength">
		''' @return </param>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="InterruptedException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int startMaster(int parameterLength, String mediaDriverDirectory) throws Exception
		Public Shared Function startMaster(ByVal parameterLength As Integer, ByVal mediaDriverDirectory As String) As Integer
			Return exec(GetType(ParameterServerSubscriber), mediaDriverDirectory, "-m", "true", "-s", "1," & parameterLength.ToString(), "-p", "40123", "-h", "localhost", "-id", "11", "-sp", "9200", "--updatesPerEpoch", "1")
		End Function


		''' <summary>
		''' Exec a java process in the background </summary>
		''' <param name="klass"> the main class to run </param>
		''' <param name="mediaDriverDirectory"> the media driver directory to use </param>
		''' <param name="args"> the args to use (can be null) </param>
		''' <returns> the process exit code </returns>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="InterruptedException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int exec(@Class klass, String mediaDriverDirectory, String... args) throws Exception
		Public Shared Function exec(ByVal klass As Type, ByVal mediaDriverDirectory As String, ParamArray ByVal args() As String) As Integer
			Dim javaHome As String = System.getProperty("java.home")
			Dim javaBin As String = javaHome & File.separator & "bin" & File.separator & "java"
			Dim classpath As String = System.getProperty("java.class.path")
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			Dim className As String = klass.FullName
			If args Is Nothing OrElse args.Length < 1 Then
				Try
					Return (New ProcessExecutor()).command(javaBin, "-cp", classpath, className).readOutput(True).redirectOutput(System.out).destroyOnExit().redirectError(System.err).execute().getExitValue()
				Catch e As TimeoutException
					log.error("",e)
				End Try
			Else
				Dim args2 As IList(Of String) = New List(Of String) From {javaBin, "-cp", classpath, className, "-md", mediaDriverDirectory}
				CType(args2, List(Of String)).AddRange(New List(Of String) From {args})
				Try
					Call (New ProcessExecutor()).command(args2).destroyOnExit().readOutput(True).redirectOutput(System.out).redirectError(System.err).execute().getExitValue()
				Catch e As TimeoutException
					log.error("",e)
				End Try
			End If


			Return 1
		End Function


	End Class

End Namespace