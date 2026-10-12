Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Pointer = org.bytedeco.javacpp.Pointer

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

Namespace org.nd4j.jita.allocator.pointers

	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @AllArgsConstructor public class PointersPair
	Public Class PointersPair
		''' <summary>
		''' this field can be null, on system without any special devices
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: private volatile org.bytedeco.javacpp.Pointer devicePointer;
		Private devicePointer As Pointer

		''' <summary>
		''' this should always contain long pointer to host memory
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: private volatile org.bytedeco.javacpp.Pointer hostPointer;
		Private hostPointer As Pointer

		Public Sub New(ByVal devicePointer As Long, ByVal hostPointer As Long)
			Me.devicePointer = New CudaPointer(devicePointer)
			Me.hostPointer = New CudaPointer(hostPointer)
		End Sub
	End Class

End Namespace