﻿Imports System
Imports PortableDataStream = org.apache.spark.input.PortableDataStream
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet

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

Namespace org.deeplearning4j.spark.parameterserver.callbacks


	Public Class MultiDataSetDeserializationCallback
		Implements PortableDataStreamMDSCallback

		Public Overridable Function compute(ByVal pds As PortableDataStream) As MultiDataSet Implements PortableDataStreamMDSCallback.compute
			Try
					Using [is] As DataInputStream = pds.open()
					' TODO: do something better here
					Dim ds As New MultiDataSet()
					ds.load([is])
					Return ds
					End Using
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function
	End Class

End Namespace