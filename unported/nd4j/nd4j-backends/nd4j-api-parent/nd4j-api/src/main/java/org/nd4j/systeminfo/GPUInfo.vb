Imports AllArgsConstructor = lombok.AllArgsConstructor
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

Namespace org.nd4j.systeminfo

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor public class GPUInfo
	Public Class GPUInfo

		Public Const fGpu As String = "  %-30s %-5s %24s %24s %24s"

		Private name As String
		Private totalMemory As Long
		Private freeMemory As Long
		Friend major As Integer
		Friend minor As Integer

		Public Overrides Function ToString() As String
			Return String.format(fGpu, name, major & "." & minor, SystemInfo.fBytes(totalMemory), SystemInfo.fBytes(totalMemory - freeMemory), SystemInfo.fBytes(freeMemory))
		End Function
	End Class

End Namespace