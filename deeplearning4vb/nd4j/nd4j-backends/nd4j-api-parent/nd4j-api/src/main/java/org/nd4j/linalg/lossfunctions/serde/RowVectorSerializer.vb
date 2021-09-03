﻿Imports System
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JsonGenerator = org.nd4j.shade.jackson.core.JsonGenerator
Imports JsonSerializer = org.nd4j.shade.jackson.databind.JsonSerializer
Imports SerializerProvider = org.nd4j.shade.jackson.databind.SerializerProvider

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

Namespace org.nd4j.linalg.lossfunctions.serde


	<Obsolete>
	Public Class RowVectorSerializer
		Inherits JsonSerializer(Of INDArray)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void serialize(org.nd4j.linalg.api.ndarray.INDArray array, org.nd4j.shade.jackson.core.JsonGenerator jsonGenerator, org.nd4j.shade.jackson.databind.SerializerProvider serializerProvider) throws java.io.IOException
		Public Overrides Sub serialize(ByVal array As INDArray, ByVal jsonGenerator As JsonGenerator, ByVal serializerProvider As SerializerProvider)
			If array.View Then
				array = array.dup()
			End If
			Dim dArr() As Double = array.data().asDouble()
			jsonGenerator.writeObject(dArr)
		End Sub
	End Class

End Namespace