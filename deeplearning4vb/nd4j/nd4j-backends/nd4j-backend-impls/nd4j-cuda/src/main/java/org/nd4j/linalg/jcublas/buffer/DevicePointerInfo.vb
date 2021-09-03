Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports PointersPair = org.nd4j.jita.allocator.pointers.PointersPair

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

Namespace org.nd4j.linalg.jcublas.buffer

	''' <summary>
	''' Provides information about a device pointer
	''' 
	''' @author bam4d
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor public class DevicePointerInfo
	Public Class DevicePointerInfo
		Private ReadOnly pointers As PointersPair
		Private ReadOnly length As Long
		Private ReadOnly stride As Integer
		Private ReadOnly offset As Integer
		Private freed As Boolean = False


	End Class

End Namespace