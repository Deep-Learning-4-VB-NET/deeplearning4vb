Imports Builder = lombok.Builder
Imports Data = lombok.Data

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

Namespace org.nd4j.aeron.ipc

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Builder public class AeronConnectionInformation
	Public Class AeronConnectionInformation
		Private connectionHost As String
		Private connectionPort As Integer
		Private streamId As Integer

		''' <summary>
		''' Traditional static generator method </summary>
		''' <param name="connectionHost"> </param>
		''' <param name="connectionPort"> </param>
		''' <param name="streamId">
		''' @return </param>
		Public Shared Function [of](ByVal connectionHost As String, ByVal connectionPort As Integer, ByVal streamId As Integer) As AeronConnectionInformation
			Return AeronConnectionInformation.builder().connectionHost(connectionHost).connectionPort(connectionPort).streamId(streamId).build()
		End Function

		Public Overrides Function ToString() As String
			Return String.Format("{0}:{1:D}:{2:D}", connectionHost, connectionPort, streamId)
		End Function
	End Class

End Namespace