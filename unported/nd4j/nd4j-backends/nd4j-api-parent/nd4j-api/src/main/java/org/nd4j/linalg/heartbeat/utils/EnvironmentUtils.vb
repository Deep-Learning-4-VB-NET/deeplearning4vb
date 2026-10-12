Imports System
Imports System.Collections.Generic
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Environment = org.nd4j.linalg.heartbeat.reports.Environment

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

Namespace org.nd4j.linalg.heartbeat.utils


	Public Class EnvironmentUtils

		''' <summary>
		''' This method build
		''' @return
		''' </summary>
		Public Shared Function buildEnvironment() As Environment
			Dim environment As New Environment()

			environment.setJavaVersion(System.getProperty("java.specification.version"))
			environment.setNumCores(Runtime.getRuntime().availableProcessors())
			environment.setAvailableMemory(Runtime.getRuntime().maxMemory())
			environment.setOsArch(System.getProperty("os.arch"))
			environment.setOsName(System.getProperty("os.opName"))
			environment.setBackendUsed(Nd4j.Executioner.GetType().Name)

			Return environment
		End Function

		Public Shared Function buildCId() As Long
	'        
	'            builds repeatable anonymous value
	'        
			Dim ret As Long = 0

			Try
				Dim interfaces As IList(Of NetworkInterface) = Collections.list(NetworkInterface.getNetworkInterfaces())

				For Each networkInterface As NetworkInterface In interfaces
					Try
						Dim arr() As SByte = networkInterface.getHardwareAddress()
						Dim seed As Long = 0
						For i As Integer = 0 To arr.Length - 1
							seed += (CLng(arr(i)) And &HffL) << (8 * i)
						Next i
						Dim random As New Random(seed)

						Return random.nextLong()
					Catch e As Exception
 ' do nothing, just skip to next interface
					End Try
				Next networkInterface

			Catch e As Exception
 ' do nothing here
			End Try

			Return ret
		End Function
	End Class

End Namespace