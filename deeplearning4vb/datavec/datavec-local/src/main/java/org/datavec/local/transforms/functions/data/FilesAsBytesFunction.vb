﻿Imports System.IO
Imports IOUtils = org.apache.commons.io.IOUtils
Imports BytesWritable = org.datavec.api.writable.BytesWritable
Imports Text = org.datavec.api.writable.Text
Imports org.nd4j.common.function
Imports org.nd4j.common.primitives

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

Namespace org.datavec.local.transforms.functions.data



	Public Class FilesAsBytesFunction
		Implements [Function](Of Pair(Of String, Stream), Pair(Of Text, BytesWritable))

		Public Overridable Function apply(ByVal [in] As Pair(Of String, Stream)) As Pair(Of Text, BytesWritable)
			Try
				Return Pair.of(New Text([in].First), New BytesWritable(IOUtils.toByteArray([in].Second)))
			Catch e As IOException
				Throw New System.InvalidOperationException(e)

			End Try

		End Function
	End Class

End Namespace