Imports Pointer = org.bytedeco.javacpp.Pointer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.nd4j.linalg.api.rng

	Public Interface Random
		Inherits AutoCloseable

		''' <summary>
		''' Sets the seed of the underlying random number generator using an
		''' <code>int</code> seed.
		''' <para>Sequences of values generated starting with the same seeds
		''' should be identical.
		''' </para>
		''' </summary>
		''' <param name="seed"> the seed value </param>
		Property Seed As Integer

		''' <summary>
		''' Sets the seed of the underlying random number generator using an
		''' <code>int</code> seed.
		''' <para>Sequences of values generated starting with the same seeds
		''' should be identical.
		''' </para>
		''' </summary>
		''' <param name="seed"> the seed value </param>
		WriteOnly Property Seed As Integer()


		''' <summary>
		''' Sets the seed of the underlying random number generator using a
		''' <code>long</code> seed.
		''' <para>Sequences of values generated starting with the same seeds
		''' should be identical.
		''' </para>
		''' </summary>
		''' <param name="seed"> the seed value </param>
		WriteOnly Property Seed As Long


		''' <summary>
		''' Generates random bytes and places them into a user-supplied
		''' byte array.  The number of random bytes produced is equal to
		''' the length of the byte array.
		''' </summary>
		''' <param name="bytes"> the non-null byte array in which to put the
		'''              random bytes </param>
		Sub nextBytes(ByVal bytes() As SByte)

		''' <summary>
		''' Returns the next pseudorandom, uniformly distributed <code>int</code>
		''' value from this random number generator's sequence.
		''' All 2<font size="-1"><sup>32</sup></font> possible <tt>int</tt> values
		''' should be produced with  (approximately) equal probability.
		''' </summary>
		''' <returns> the next pseudorandom, uniformly distributed <code>int</code>
		''' value from this random number generator's sequence </returns>
		Function nextInt() As Integer

		''' <summary>
		''' Returns a pseudorandom, uniformly distributed <tt>int</tt> value
		''' between 0 (inclusive) and the specified value (exclusive), drawn from
		''' this random number generator's sequence.
		''' </summary>
		''' <param name="n"> the bound on the random number to be returned.  Must be
		'''          positive. </param>
		''' <returns> a pseudorandom, uniformly distributed <tt>int</tt>
		''' value between 0 (inclusive) and n (exclusive). </returns>
		''' <exception cref="IllegalArgumentException"> if n is not positive. </exception>
		Function nextInt(ByVal n As Integer) As Integer


		''' <summary>
		''' Returns a pseudorandom, uniformly distributed <tt>int</tt> value
		''' between 0 (inclusive) and the specified value (exclusive), drawn from
		''' this random number generator's sequence.
		''' </summary>
		''' <param name="n"> the bound on the random number to be returned.  Must be
		'''          positive. </param>
		''' <returns> a pseudorandom, uniformly distributed <tt>int</tt>
		''' value between 0 (inclusive) and n (exclusive). </returns>
		''' <exception cref="IllegalArgumentException"> if n is not positive. </exception>
		Function nextInt(ByVal a As Integer, ByVal n As Integer) As Integer

		''' <summary>
		''' Returns the next pseudorandom, uniformly distributed <code>long</code>
		''' value from this random number generator's sequence.  All
		''' 2<font size="-1"><sup>64</sup></font> possible <tt>long</tt> values
		''' should be produced with (approximately) equal probability.
		''' </summary>
		''' <returns> the next pseudorandom, uniformly distributed <code>long</code>
		''' value from this random number generator's sequence </returns>
		Function nextLong() As Long

		''' <summary>
		''' Returns the next pseudorandom, uniformly distributed
		''' <code>boolean</code> value from this random number generator's
		''' sequence.
		''' </summary>
		''' <returns> the next pseudorandom, uniformly distributed
		''' <code>boolean</code> value from this random number generator's
		''' sequence </returns>
		Function nextBoolean() As Boolean

		''' <summary>
		''' Returns the next pseudorandom, uniformly distributed <code>float</code>
		''' value between <code>0.0</code> and <code>1.0</code> from this random
		''' number generator's sequence.
		''' </summary>
		''' <returns> the next pseudorandom, uniformly distributed <code>float</code>
		''' value between <code>0.0</code> and <code>1.0</code> from this
		''' random number generator's sequence </returns>
		Function nextFloat() As Single

		''' <summary>
		''' Returns the next pseudorandom, uniformly distributed
		''' <code>double</code> value between <code>0.0</code> and
		''' <code>1.0</code> from this random number generator's sequence.
		''' </summary>
		''' <returns> the next pseudorandom, uniformly distributed
		''' <code>double</code> value between <code>0.0</code> and
		''' <code>1.0</code> from this random number generator's sequence </returns>
		Function nextDouble() As Double

		''' <summary>
		''' Returns the next pseudorandom, Gaussian ("normally") distributed
		''' <code>double</code> value with mean <code>0.0</code> and standard
		''' deviation <code>1.0</code> from this random number generator's sequence.
		''' </summary>
		''' <returns> the next pseudorandom, Gaussian ("normally") distributed
		''' <code>double</code> value with mean <code>0.0</code> and
		''' standard deviation <code>1.0</code> from this random number
		''' generator's sequence </returns>
		Function nextGaussian() As Double

		''' <summary>
		''' Generate a gaussian number ndarray
		''' of the specified shape
		''' </summary>
		''' <param name="shape"> the shape to generate </param>
		''' <returns> the generated gaussian numbers </returns>
		Function nextGaussian(ByVal shape() As Integer) As INDArray


		Function nextGaussian(ByVal shape() As Long) As INDArray

		''' <summary>
		''' Generate a gaussian number ndarray
		''' of the specified shape and order
		''' </summary>
		''' <param name="order"> the order of the output array </param>
		''' <param name="shape"> the shape to generate </param>
		''' <returns> the generated gaussian numbers </returns>
		Function nextGaussian(ByVal order As Char, ByVal shape() As Integer) As INDArray


		Function nextGaussian(ByVal order As Char, ByVal shape() As Long) As INDArray

		''' <summary>
		''' Generate a uniform number ndarray
		''' of the specified shape
		''' </summary>
		''' <param name="shape"> the shape to generate </param>
		''' <returns> the generated gaussian numbers </returns>
		Function nextDouble(ByVal shape() As Integer) As INDArray

		Function nextDouble(ByVal shape() As Long) As INDArray

		''' <summary>
		''' Generate a uniform number ndarray
		''' of the specified shape and order
		''' </summary>
		''' <param name="order"> order of the output array </param>
		''' <param name="shape"> the shape to generate </param>
		''' <returns> the generated gaussian numbers </returns>
		Function nextDouble(ByVal order As Char, ByVal shape() As Integer) As INDArray


		Function nextDouble(ByVal order As Char, ByVal shape() As Long) As INDArray

		''' <summary>
		''' Generate a uniform number ndarray
		''' of the specified shape
		''' </summary>
		''' <param name="shape"> the shape to generate </param>
		''' <returns> the generated uniform numbers </returns>
		Function nextFloat(ByVal shape() As Integer) As INDArray


		Function nextFloat(ByVal shape() As Long) As INDArray

		''' <summary>
		''' Generate a uniform number ndarray
		''' of the specified shape
		''' </summary>
		''' <param name="shape"> the shape to generate </param>
		''' <returns> the generated uniform numbers </returns>
		Function nextFloat(ByVal order As Char, ByVal shape() As Integer) As INDArray

		Function nextFloat(ByVal order As Char, ByVal shape() As Long) As INDArray

		''' <summary>
		''' Generate a random set of integers
		''' of the specified shape. Note that
		''' these integers will not actually be integers
		''' but floats that happen to be whole numbers.
		''' The reason for this is due to ints
		''' having the same space usage as floats.
		''' This also plays nice with blas.
		''' <p/>
		''' If the data opType is set to double,
		''' then these will be whole doubles.
		''' </summary>
		''' <param name="shape"> the shape to generate </param>
		''' <returns> the ndarray with
		''' the shape of only integers. </returns>
		Function nextInt(ByVal shape() As Integer) As INDArray

		Function nextInt(ByVal shape() As Long) As INDArray


		''' <summary>
		''' Generate a random set of integers
		''' of the specified shape. Note that
		''' these integers will not actually be integers
		''' but floats that happen to be whole numbers.
		''' The reason for this is due to ints
		''' having the same space usage as floats.
		''' This also plays nice with blas.
		''' <p/>
		''' If the data opType is set to double,
		''' then these will be whole doubles.
		''' </summary>
		''' <param name="shape"> the shape to generate </param>
		''' <param name="n">     the max number to generate trod a </param>
		''' <returns> the ndarray with
		''' the shape of only integers. </returns>
		Function nextInt(ByVal n As Integer, ByVal shape() As Integer) As INDArray

		Function nextInt(ByVal n As Integer, ByVal shape() As Long) As INDArray

		''' <summary>
		''' This method returns pointer to RNG state structure.
		''' Please note: DefaultRandom implementation returns NULL here, making it impossible to use with RandomOps
		''' 
		''' @return
		''' </summary>
		ReadOnly Property StatePointer As Pointer

		''' <summary>
		''' This method returns number of elements consumed
		''' @return
		''' </summary>
		ReadOnly Property Position As Long

		''' <summary>
		''' This method is similar to setSeed() but it doesn't really touches underlying buffer, if any. So it acts as additional modifier to current RNG state. System.currentTimeMillis() will be used as modifier.
		''' 
		''' PLEASE NOTE: Never use this method unless you're 100% sure what it does and why you would need it.
		''' 
		''' </summary>
		Sub reSeed()

		''' <summary>
		''' This method is similar to setSeed() but it doesn't really touches underlying buffer, if any. So it acts as additional modifier to current RNG state.
		''' 
		''' PLEASE NOTE: Never use this method unless you're 100% sure what it does and why you would need it.
		''' </summary>
		''' <param name="seed"> </param>
		Sub reSeed(ByVal seed As Long)

		Function rootState() As Long

		Function nodeState() As Long

		Sub setStates(ByVal rootState As Long, ByVal nodeState As Long)
	End Interface



End Namespace