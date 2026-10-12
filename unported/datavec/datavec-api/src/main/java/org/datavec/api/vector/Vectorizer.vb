Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports RecordReader = org.datavec.api.records.reader.RecordReader

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

Namespace org.datavec.api.vector

	Public Interface Vectorizer(Of VECTOR_TYPE)


		''' <summary>
		''' Create a vector based on the given arguments </summary>
		''' <param name="args"> the arguments to create a vector with </param>
		''' <returns> the created vector
		'''  </returns>
		Function createVector(ByVal args() As Object) As VECTOR_TYPE

		''' <summary>
		''' Initialize based on a configuration </summary>
		''' <param name="conf"> the configuration to use </param>
		Sub initialize(ByVal conf As Configuration)

		''' <summary>
		''' Fit based on a record reader </summary>
		''' <param name="reader"> </param>
		Sub fit(ByVal reader As RecordReader)

		''' <summary>
		''' Fit based on a record reader </summary>
		''' <param name="reader"> </param>
		Function fitTransform(ByVal reader As RecordReader) As VECTOR_TYPE


		''' <summary>
		''' Fit based on a record reader </summary>
		''' <param name="reader"> </param>
		''' <param name="callBack"> </param>
		Sub fit(ByVal reader As RecordReader, ByVal callBack As RecordCallBack)

		''' <summary>
		''' Fit based on a record reader </summary>
		''' <param name="reader"> </param>
		''' <param name="callBack"> </param>
		Function fitTransform(ByVal reader As RecordReader, ByVal callBack As RecordCallBack) As VECTOR_TYPE

		''' <summary>
		''' Transform a record in to a vector </summary>
		''' <param name="record"> the record to write
		''' @return </param>
		Function transform(ByVal record As Record) As VECTOR_TYPE


		''' <summary>
		''' On record call back.
		''' This allows for neat inheritance and polymorphism
		''' for fit and fit/transform among other things
		''' </summary>
		Public Interface RecordCallBack
			''' <summary>
			''' The record callback </summary>
			''' <param name="record"> </param>
			Sub onRecord(ByVal record As Record)
		End Interface


	End Interface

End Namespace