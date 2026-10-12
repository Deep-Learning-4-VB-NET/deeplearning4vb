Imports System
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports TransformProcessRecordReader = org.datavec.api.records.reader.impl.transform.TransformProcessRecordReader
Imports TransformProcess = org.datavec.api.transform.TransformProcess

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

Namespace org.datavec.local.transforms

	<Serializable>
	Public Class LocalTransformProcessRecordReader
		Inherits TransformProcessRecordReader

		''' <summary>
		''' Initialize with the internal record reader
		''' and the transform process. </summary>
		''' <param name="recordReader"> </param>
		''' <param name="transformProcess"> </param>
		Public Sub New(ByVal recordReader As RecordReader, ByVal transformProcess As TransformProcess)
			MyBase.New(recordReader, transformProcess)
		End Sub
	End Class

End Namespace