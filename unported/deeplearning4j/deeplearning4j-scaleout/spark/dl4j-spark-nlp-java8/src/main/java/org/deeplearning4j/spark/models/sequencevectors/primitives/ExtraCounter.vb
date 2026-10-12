Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.nd4j.common.primitives
Imports NetworkInformation = org.nd4j.parameterserver.distributed.util.NetworkInformation

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

Namespace org.deeplearning4j.spark.models.sequencevectors.primitives


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Slf4j public class ExtraCounter<E> extends org.nd4j.common.primitives.Counter<E>
	<Serializable>
	Public Class ExtraCounter(Of E)
		Inherits Counter(Of E)

		Private networkInformation As ISet(Of NetworkInformation)

		Public Sub New()
			MyBase.New()
			networkInformation = New HashSet(Of NetworkInformation)()
		End Sub

		Public Overridable Sub buildNetworkSnapshot()
			Try
				Dim netInfo As New NetworkInformation()
				netInfo.setTotalMemory(Runtime.getRuntime().maxMemory())
				netInfo.setAvailableMemory(Runtime.getRuntime().freeMemory())

				Dim sparkIp As String = Environment.GetEnvironmentVariable("SPARK_PUBLIC_DNS")
				If sparkIp IsNot Nothing Then
					' if spark ip is defined, we just use it, and don't bother with other interfaces

					netInfo.addIpAddress(sparkIp)
				Else
					' sparkIp wasn't defined, so we'll go for heuristics here
					Dim interfaces As IList(Of NetworkInterface) = Collections.list(NetworkInterface.getNetworkInterfaces())

					For Each networkInterface As NetworkInterface In interfaces
						If networkInterface.isLoopback() OrElse Not networkInterface.isUp() Then
							Continue For
						End If

						For Each address As InterfaceAddress In networkInterface.getInterfaceAddresses()
							Dim addr As String = address.getAddress().getHostAddress()

							If addr Is Nothing OrElse addr.Length = 0 OrElse addr.Contains(":") Then
								Continue For
							End If

							netInfo.getIpAddresses().add(addr)
						Next address
					Next networkInterface
				End If
				networkInformation.Add(netInfo)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		Public Overrides Sub incrementAll(Of T As E)(ByVal counter As Counter(Of T))
			If TypeOf counter Is ExtraCounter Then
				networkInformation.addAll(CType(counter, ExtraCounter).networkInformation)
			End If

			MyBase.incrementAll(counter)
		End Sub
	End Class

End Namespace