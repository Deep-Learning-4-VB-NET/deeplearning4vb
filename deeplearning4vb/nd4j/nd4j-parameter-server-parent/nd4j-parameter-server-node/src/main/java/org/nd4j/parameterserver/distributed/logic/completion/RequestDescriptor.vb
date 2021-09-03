Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor

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

Namespace org.nd4j.parameterserver.distributed.logic.completion

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @AllArgsConstructor @Deprecated public class RequestDescriptor
	<Obsolete>
	Public Class RequestDescriptor
		Private originatorId As Long
		Private taskId As Long


		Public Shared Function createDescriptor(ByVal originatorId As Long, ByVal taskId As Long) As RequestDescriptor
			Return New RequestDescriptor(originatorId, taskId)
		End Function
	End Class

End Namespace